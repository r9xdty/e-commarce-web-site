using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using CategoriesWithProducts.UI.Models.DTO;
using System.Net.Http.Headers;

namespace CategoriesWithProducts.UI.Controllers
{
    public class OrderController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public OrderController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        
        public async Task<IActionResult> MyOrders()
        {
            var token = HttpContext.Session.GetString("token");

            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Please Login to Access Your Orders";
                return RedirectToAction("Login", "Auth");
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("https://localhost:7287/api/order/user");

            if (!response.IsSuccessStatusCode)
                return View(new List<OrderDto>());

            var content = await response.Content.ReadAsStringAsync();
            var orders = JsonSerializer.Deserialize<List<OrderDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(orders);
        }


        public async Task<IActionResult> AllOrders()
        {
            var token = HttpContext.Session.GetString("token");
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "You can't access this page";
                return RedirectToAction("Login", "Auth");
            }

            if (!User.IsInRole("Admin"))
            {
                TempData["ErrorMessage"] = "You can't access this page";
                return RedirectToAction("AccessDenied", "Auth");
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("https://localhost:7287/api/order");

            if (!response.IsSuccessStatusCode)
            {
                return View(new Dictionary<Guid, List<OrderDto>>());
            }

            var content = await response.Content.ReadAsStringAsync();
            var orders = JsonSerializer.Deserialize<List<OrderDto>>(content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var groupedOrders = orders.GroupBy(o => o.UserId).ToDictionary(g => g.Key, g => g.ToList());

            return View(groupedOrders);
        }



        [HttpPost]
        public async Task<IActionResult> Refund(RefundOrderItemDto dto)
        {
            var token = HttpContext.Session.GetString("token");
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Please Login to Make a Refund";
                return RedirectToAction("Login", "Auth");
            }
                

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonSerializer.Serialize(dto), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://localhost:7287/api/order/refund", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Refunded Successfuly.";
            }
            else
            {
                TempData["ErrorMessage"] = "Refund Failed.";
            }
              
            return RedirectToAction("MyOrders");
        }
    }
}
