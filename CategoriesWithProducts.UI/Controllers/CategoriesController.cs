using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CategoriesWithProducts.UI.Models;
using CategoriesWithProducts.UI.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CategoriesWithProducts.UI.Controllers
{

    public class CategoriesController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public CategoriesController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<CategoryDto> response = new List<CategoryDto>();
            try
            {
                var token = HttpContext.Session.GetString("token");
                var client = httpClientFactory.CreateClient();

                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }


                var httpResponseMessage = await client.GetAsync("https://localhost:7287/api/category");

                httpResponseMessage.EnsureSuccessStatusCode();

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    response.AddRange(await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<CategoryDto>>());
                }
            }
            catch (Exception ex)
            {


            }

            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] AddCategoryViewModel model)
        {
            var token = HttpContext.Session.GetString("token");
            var client = httpClientFactory.CreateClient();

            if (!string.IsNullOrEmpty(token))
            {
                TempData["SuccessMessage"] = "Category added successfully!";
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
                RequestUri = new Uri("https://localhost:7287/api/category"),
                Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
            };

            var httpResponseMessage = await client.SendAsync(httpRequestMessage);

            httpResponseMessage.EnsureSuccessStatusCode();

            var response = await httpResponseMessage.Content.ReadFromJsonAsync<CategoryDto>();

            if (response is not null)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(Guid id)
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

            var response = await client.GetFromJsonAsync<CategoryDto>($"https://localhost:7287/api/category/{id.ToString()}");

            if (response is not null)
            {
                return Json(response);
            }

            return Json(new { success = false });
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory([FromBody] CategoryDto request)
        {
            var token = HttpContext.Session.GetString("token");
            var client = httpClientFactory.CreateClient();

            if (!string.IsNullOrEmpty(token))
            {
                TempData["SuccessMessage"] = "Category edited successfully!";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                TempData["Error"] = "Unauthorized access.";
                return RedirectToAction("Login", "Auth");
            }

            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Put,
                RequestUri = new Uri($"https://localhost:7287/api/category/{request.Id}"),
                Content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
            };

            var httpResponseMessage = await client.SendAsync(httpRequestMessage);
            httpResponseMessage.EnsureSuccessStatusCode();

            var response = await httpResponseMessage.Content.ReadFromJsonAsync<CategoryDto>();
            if (response is not null)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCategory(Guid id)
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

            var response = await client.GetFromJsonAsync<CategoryDto>($"https://localhost:7287/api/category/{id.ToString()}");

            if (response is not null)
            {
                return View(response);
            }

            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(CategoryDto request)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                var client = httpClientFactory.CreateClient();

                if (!string.IsNullOrEmpty(token))
                {
                    TempData["SuccessMessage"] = "Category deleted successfully!";
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                else
                {
                    TempData["Error"] = "Unauthorized access.";
                    return RedirectToAction("Login", "Auth");
                }

                var httpResponseMessage = await client.DeleteAsync($"https://localhost:7287/api/category/{request.Id}");

                httpResponseMessage.EnsureSuccessStatusCode();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {


            }
            return Json(new { success = false });
        }
    }
}
