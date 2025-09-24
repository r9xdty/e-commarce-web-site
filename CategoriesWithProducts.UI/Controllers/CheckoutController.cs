using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CategoriesWithProducts.UI.Helpers;
using CategoriesWithProducts.UI.Models;
using CategoriesWithProducts.UI.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace CategoriesWithProducts.UI.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public CheckoutController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Checkout(string? code = null)
        {
            var cart = CookieHelper.GetCart(HttpContext);
            var token = HttpContext.Session.GetString("token");
            var productTotal = cart.Items.Sum(i => i.ProductPrice * i.Quantity);
            var freeShipping = productTotal >= 500;
            var shippingPrice = freeShipping ? 0 : 50;

            decimal discountAmount = 0;
            var totalPrice = productTotal;

            CouponDto appliedCoupon = null;

            if (!string.IsNullOrEmpty(code))
            {
                var existingCouponJson = TempData["AppliedCoupon"] as string;
                var existingCoupon = !string.IsNullOrEmpty(existingCouponJson)
                    ? JsonSerializer.Deserialize<CouponDto>(existingCouponJson)
                    : null;

                if (existingCoupon?.Code == code)
                {
                    appliedCoupon = existingCoupon;
                    discountAmount = appliedCoupon.IsPercent
                        ? productTotal * appliedCoupon.DiscountRate / 100
                        : appliedCoupon.DiscountRate;

                    totalPrice -= discountAmount;

                    TempData["Error"] = "You are already using this code!";
                    TempData.Keep("AppliedCoupon");
                }
                else
                {
                    var client = httpClientFactory.CreateClient();

                    if (!string.IsNullOrEmpty(token))
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }

                    if (totalPrice < 50)
                    {
                        TempData["Error"] = "Cart cannot be belove 50 ₺";
                        return RedirectToAction("Checkout", "Checkout");
                    }

                        var httpRequestMessage = new HttpRequestMessage()
                        {
                            Method = HttpMethod.Post,
                            RequestUri = new Uri("https://localhost:7287/api/coupons/apply"),
                            Content = new StringContent(JsonSerializer.Serialize(new ApplyCouponViewModel { Code = code }), Encoding.UTF8, "application/json")
                        };

                    var httpResponseMessage = await client.SendAsync(httpRequestMessage);

                    if (httpResponseMessage.IsSuccessStatusCode)
                    {


                        appliedCoupon = await httpResponseMessage.Content.ReadAsAsync<CouponDto>();

                        if (appliedCoupon != null && appliedCoupon.ExpirationDate > DateTime.Now)
                        {
                            if (appliedCoupon.IsPercent)
                            {
                                discountAmount = productTotal * appliedCoupon.DiscountRate / 100;
                            }
                            else
                            {
                                discountAmount = appliedCoupon.DiscountRate;
                            }
                            totalPrice -= discountAmount;
                            TempData["AppliedCoupon"] = JsonSerializer.Serialize(appliedCoupon);
                        }
                        TempData["SuccessMessage"] = "Discount Applied Successfuly!";

                    }
                    else if (string.IsNullOrEmpty(token))
                    {
                        TempData["Error"] = "Please Login to use coupons!";
                        return RedirectToAction("Login", "Auth", new { ReturnUrl = Url.Action("Checkout", "Checkout") });
                    }
                    else
                    {
                        TempData["Error"] = "Discount Code not Valid!";
                        TempData.Remove("AppliedCoupon");
                    }
                }
            }
            else if (TempData["AppliedCoupon"] is string couponJson)
            {
                appliedCoupon = JsonSerializer.Deserialize<CouponDto>(couponJson);
                if (appliedCoupon != null)
                {
                    if (appliedCoupon.IsPercent)
                    {
                        discountAmount = productTotal * appliedCoupon.DiscountRate / 100;
                    }
                    else
                    {
                        discountAmount = appliedCoupon.DiscountRate;
                    }
                    totalPrice -= discountAmount;
                    TempData.Keep("AppliedCoupon");
                }
            }

            totalPrice += shippingPrice;

            var model = new CheckoutViewModel
            {
                CartItems = cart.Items,
                ProductTotalPrice = productTotal,
                ShippingPrice = shippingPrice,
                TotalPrice = totalPrice,
                FreeShipping = freeShipping,
                AppliedCoupon = appliedCoupon,
                DiscountAmount = discountAmount
            };

            return View(model);


        }

        [HttpPost]
        public async Task<IActionResult> CheckoutConfirmed()
        {
            var token = HttpContext.Session.GetString("token");
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Please login to order";
                return RedirectToAction("Login", "Auth", new { ReturnUrl = Url.Action("Checkout", "Checkout") });
            }

            var cart = CookieHelper.GetCart(HttpContext);

            if (cart == null || !cart.Items.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("Checkout");
            }

            var orderDto = new AddOrderDto
            {
                OrderItems = cart.Items.Select(item => new AddOrderItemDto
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                }).ToList()
            };

            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonSerializer.Serialize(orderDto), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7287/api/order", content);

            if (response.IsSuccessStatusCode)
            {
                CookieHelper.ClearCart(HttpContext);
                TempData.Remove("AppliedCoupon");
                TempData["SuccessMessage"] = "We took your order successfully";
                return RedirectToAction("MyOrders", "Order");
            }
            else
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = "Order failed: " + errorMsg;
                return RedirectToAction("Checkout");
            }
        }

        public async Task<IActionResult> RemoveCoupon(string code)
        {
            var token = HttpContext.Session.GetString("token");
            var client = httpClientFactory.CreateClient();

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"https://localhost:7287/api/coupons/unapply?code={code}")
            };

            var response = await client.SendAsync(httpRequestMessage);

            if (response.IsSuccessStatusCode)
            {
                TempData.Remove("AppliedCoupon");
                TempData["SuccessMessage"] = "Coupon removed successfully!";
                await Checkout();
                return View("Checkout");
            }
            else
            {
                // Hata mesajı loglanabilir ya da kullanıcıya gösterilebilir
                var error = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Failed to remove coupon: {error}";
                return RedirectToAction("Checkout");
            }
        }
    }
}
