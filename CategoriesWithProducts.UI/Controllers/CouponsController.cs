using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using CategoriesWithProducts.UI.Helpers;
using CategoriesWithProducts.UI.Models;
using CategoriesWithProducts.UI.Models.DTO;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CategoriesWithProducts.UI.Controllers
{
    public class CouponsController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public CouponsController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> ShowAllCoupons()
        {
            List<CouponDto> response = new List<CouponDto>();

            try
            {
                var token = HttpContext.Session.GetString("token");
                var client = httpClientFactory.CreateClient();

                if (!string.IsNullOrEmpty(token) && User.IsInRole("Admin"))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                else
                {
                    TempData["SuccessMessage"] = "You can't access this page";
                    return RedirectToAction("AccessDenied", "Auth");
                }


                var httpResponseMessage = await client.GetAsync("https://localhost:7287/api/coupons");

                httpResponseMessage.EnsureSuccessStatusCode();

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    response.AddRange(await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<CouponDto>>());
                }
            }
            catch (Exception ex)
            {


            }

            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddCoupon([FromBody] AddCouponViewModel addCoupon)
        {
            var token = HttpContext.Session.GetString("token");
            var client = httpClientFactory.CreateClient();

            if (!string.IsNullOrEmpty(token) && User.IsInRole("Admin"))
            {
                TempData["SuccessMessage"] = "Coupon added successfully!";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                TempData["Error"] = "Unauthorized access.";
                return RedirectToAction("Login", "Auth");
            }

            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://localhost:7287/api/coupons"),
                Content = new StringContent(JsonSerializer.Serialize(addCoupon), Encoding.UTF8, "application/json")
            };

            var httpResponseMessage = await client.SendAsync(httpRequestMessage);

            httpResponseMessage.EnsureSuccessStatusCode();

            var response = await httpResponseMessage.Content.ReadFromJsonAsync<CouponDto>();

            if (response is not null)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpGet]
        public async Task<IActionResult> RemoveCoupon(Guid id)
        {
            var token = HttpContext.Session.GetString("token");
            var client = httpClientFactory.CreateClient();

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                TempData["Error"] = "Unauthorized access.";
                return RedirectToAction("Login", "Auth");
            }

            var response = await client.GetFromJsonAsync<CouponDto>($"https://localhost:7287/api/coupons/{id.ToString()}");

            if (response is not null)
            {
                return View(response);
            }

            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CouponDto request)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                var client = httpClientFactory.CreateClient();

                if (!string.IsNullOrEmpty(token))
                {
                    TempData["SuccessMessage"] = "Coupon deleted successfully!";
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                else
                {
                    TempData["Error"] = "Unauthorized access.";
                    return RedirectToAction("Login", "Auth");
                }

                var httpResponseMessage = await client.DeleteAsync($"https://localhost:7287/api/coupons/{request.Id}");

                httpResponseMessage.EnsureSuccessStatusCode();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {


            }
            return Json(new { success = false });
        }

        [HttpGet]
        public async Task<IActionResult> ShowCouponsPerUser()
        {
            List<UserCouponDto> response = new List<UserCouponDto>();

            try
            {
                var token = HttpContext.Session.GetString("token");
                var client = httpClientFactory.CreateClient();

                if (!string.IsNullOrEmpty(token) && User.IsInRole("Admin"))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                else
                {
                    TempData["SuccessMessage"] = "You can't access this page";
                    return RedirectToAction("AccessDenied", "Auth");
                }

                var httpResponseMessage = await client.GetAsync("https://localhost:7287/api/coupons/used-by-users");

                httpResponseMessage.EnsureSuccessStatusCode();

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    response.AddRange(await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<UserCouponDto>>());
                }
            }
            catch (Exception ex)
            {


            }

            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCouponCodeFromUser([FromBody] DeleteUserCouponViewModel model)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                var client = httpClientFactory.CreateClient();

                if (!string.IsNullOrEmpty(token))
                {
                    TempData["SuccessMessage"] = "Coupon unapplied successfully!";
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                else
                {
                    TempData["Error"] = "Unauthorized access.";
                    return RedirectToAction("Login", "Auth");
                }
                var httpResponseMessage = await client.DeleteAsync($"https://localhost:7287/api/coupons/unapply-by-admin?userId={model.UserId}&code={model.CouponCode}");
                httpResponseMessage.EnsureSuccessStatusCode();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
