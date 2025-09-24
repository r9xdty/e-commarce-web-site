using System.Drawing.Printing;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CategoriesWithProducts.UI.Models;
using CategoriesWithProducts.UI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CategoriesWithProducts.UI.Controllers
{
    public class ProductController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ProductController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CategoryDto categoryDto, string? filterQuery = null,
        decimal? minValue = null, decimal? maxValue = null, string? sortBy = null, bool? isAscending = true,
        int pageNumber = 1, int pageSize = 20)
        {

            List<ProductDto> response = new List<ProductDto>();
            try
            {


                var token = HttpContext.Session.GetString("token");
                var client = httpClientFactory.CreateClient();

                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }


                // API'ye Query String ile istek at
                var url = $"https://localhost:7287/api/products/{categoryDto.Id}/products?";

                var queryParams = new List<string>();

                if (!string.IsNullOrWhiteSpace(filterQuery))
                    queryParams.Add($"filterQuery={filterQuery}");

                if (minValue.HasValue)
                    queryParams.Add($"&minValue={minValue}");

                if (maxValue.HasValue)
                    queryParams.Add($"&maxValue={maxValue}");

                if (!string.IsNullOrWhiteSpace(sortBy))
                    queryParams.Add($"&sortBy={sortBy}&isAscending={isAscending}");

                queryParams.Add($"&pageNumber={pageNumber}&pageSize={pageSize}");

                if (queryParams.Any())
                {
                    url += string.Join("&", queryParams);
                }

                var httpResponseMessage = await client.GetAsync(url);

                httpResponseMessage.EnsureSuccessStatusCode();

                response.AddRange(await httpResponseMessage.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>());

                ViewBag.CategoryId = categoryDto.Id;

                string ascending = "";

                string activeFilter = $"";
                if (!string.IsNullOrWhiteSpace(filterQuery))
                {
                    activeFilter += $"searched for: {filterQuery}";
                }
                if ((minValue.HasValue || maxValue.HasValue))
                {
                    if (!string.IsNullOrWhiteSpace(filterQuery))
                    {
                        activeFilter += ", ";
                    }
                    activeFilter += $"Price Range: {minValue} - {maxValue}";
                }

                if (sortBy == "Sort By Name" || sortBy == "Sort By Price")
                {
                    if (isAscending == true)
                    {
                        ascending = "Ascending";
                    }
                    else
                    {
                        ascending = "Decending";
                    }
                    if (!string.IsNullOrWhiteSpace(filterQuery) || (!string.IsNullOrWhiteSpace(filterQuery)))
                    {
                        activeFilter += ", ";
                    }
                    activeFilter += $"Sorted By: {sortBy} - {ascending}";
                }
                if (activeFilter == "")
                {
                    activeFilter = "There is no filter applied.";
                }

                ViewBag.ActiveFilter = activeFilter;

                return View(response);
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] AddProductViewModel model)
        {
            var token = HttpContext.Session.GetString("token");
            var client = httpClientFactory.CreateClient();

            if (!string.IsNullOrEmpty(token))
            {
                TempData["SuccessMessage"] = "Product added successfully!";
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                TempData["Error"] = "Unauthorized access.";
                return RedirectToAction("Login", "Auth");
            }

            var categoryId = model.CategoryId;

            var httpRequestMessage = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"https://localhost:7287/api/products/{categoryId}"),
                Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
            };

            var httpResponseMessage = await client.SendAsync(httpRequestMessage);

            httpResponseMessage.EnsureSuccessStatusCode();

            var response = await httpResponseMessage.Content.ReadFromJsonAsync<ProductDto>();

            if (response is not null)
            {
                //return RedirectToAction("Index", "Product", new { id = model.CategoryId });
                return Json(new { success = true });
            }
            //return View();
            return Json(new { success = false });
        }


        [HttpGet]
        public async Task<IActionResult> EditProduct(Guid id)
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

            var response = await client.GetFromJsonAsync<ProductDto>($"https://localhost:7287/api/products/{id}");

            if (response is not null)
            {
                return Json(response);
            }

            return Json(new { success = false });
        }

        [HttpGet]
        public async Task<JsonResult> LiveSearch(string term)
        {
            var client = httpClientFactory.CreateClient();
            var response = await client.GetAsync($"https://localhost:7287/api/Products/search?query={term}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<ProductDto>>(jsonData);
                return Json(products);
            }

            return Json(new List<ProductDto>());
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct([FromBody] ProductDto request)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                var client = httpClientFactory.CreateClient();

                if (!string.IsNullOrEmpty(token))
                {
                    TempData["SuccessMessage"] = "Product edited successfully!";
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
                    RequestUri = new Uri($"https://localhost:7287/api/products/{request.Id}"),
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(request), Encoding.UTF8, "application/json")
                };

                var httpResponseMessage = await client.SendAsync(httpRequestMessage);
                httpResponseMessage.EnsureSuccessStatusCode();

                var response = await httpResponseMessage.Content.ReadFromJsonAsync<ProductDto>();
                if (response is not null)
                {
                    return Json(new { success = true });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = false, message = "Unexpected error occurred." });
        }


        [HttpPost]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            try
            {
                var token = HttpContext.Session.GetString("token");
                var client = httpClientFactory.CreateClient();

                if (!string.IsNullOrEmpty(token))
                {
                    TempData["SuccessMessage"] = "Product deleted successfully!";
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                else
                {
                    TempData["Error"] = "Unauthorized access.";
                    return RedirectToAction("Login", "Auth");
                }
                ;

                var httpResponseMessage = await client.DeleteAsync($"https://localhost:7287/api/products/{id}");

                httpResponseMessage.EnsureSuccessStatusCode();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ShowAllProducts(string? filterQuery = null,
            decimal? minValue = null, decimal? maxValue = null, string? sortBy = null,
            bool isAscending = true, int pageNumber = 1, int pageSize = 10)
        {
            List<ProductDto> response = new List<ProductDto>();
            try
            {
                var token = HttpContext.Session.GetString("token");
                var client = httpClientFactory.CreateClient();

                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                // API Query
                var queryParams = new List<string>();

                queryParams.Add("filterOn=Name");

                if (!string.IsNullOrWhiteSpace(filterQuery))
                    queryParams.Add($"filterQuery={filterQuery}");

                if (minValue.HasValue)
                    queryParams.Add($"minValue={minValue}");

                if (maxValue.HasValue)
                    queryParams.Add($"maxValue={maxValue}");

                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    queryParams.Add($"sortBy={sortBy}");
                    queryParams.Add($"isAscending={isAscending}");
                }

                queryParams.Add($"pageNumber={pageNumber}");
                queryParams.Add($"pageSize={pageSize}");

                var url = $"https://localhost:7287/api/products?{string.Join("&", queryParams)}";

                var httpResponseMessage = await client.GetAsync(url);
                httpResponseMessage.EnsureSuccessStatusCode();

                var pagedResult = await httpResponseMessage.Content.ReadFromJsonAsync<PagedProductResultDto>();
                var products = pagedResult.Products;
                var totalCount = pagedResult.TotalCount;

                response.AddRange(products);

                ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                // Filtre bilgisini ViewBag’e aktar
                string activeFilter = "";

                if (!string.IsNullOrWhiteSpace(filterQuery))
                    activeFilter += $"Searched for: {filterQuery}";

                if (minValue.HasValue || maxValue.HasValue)
                {
                    if (activeFilter != "") activeFilter += ", ";
                    activeFilter += $"Price Range: {minValue ?? 0} - {maxValue ?? decimal.MaxValue}";
                }

                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    if (activeFilter != "") activeFilter += ", ";
                    activeFilter += $"Sorted by: {sortBy} - {(isAscending ? "Ascending" : "Descending")}";
                }

                if (string.IsNullOrWhiteSpace(activeFilter))
                    activeFilter = "There is no filter applied.";

                ViewBag.ActiveFilter = activeFilter;

                ViewBag.CurrentPage = pageNumber;
                ViewBag.TotalPages = 10; // Örnek, API response’tan alınmalı
                ViewBag.CurrentFilterQuery = filterQuery;
                ViewBag.CurrentMinValue = minValue;
                ViewBag.CurrentMaxValue = maxValue;
                ViewBag.CurrentSortBy = sortBy;
                ViewBag.CurrentIsAscending = isAscending;

                return View(response);
            }
            catch (Exception ex)
            {
                return View("Error", ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchProducts(string filterQuery)
        {
            var token = HttpContext.Session.GetString("token");
            var client = httpClientFactory.CreateClient();

            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var url = $"https://localhost:7287/api/products?filterOn=Name&filterQuery={filterQuery}";

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var pagedResult = await response.Content.ReadFromJsonAsync<PagedProductResultDto>();
            var products = pagedResult.Products;

            return PartialView("_ProductCardsPartial", products);
        }

        [HttpGet]
        public async Task<IActionResult> SelectedProduct(Guid id)
        {
            var client = httpClientFactory.CreateClient();
            var url = $"https://localhost:7287/api/products/{id}";
            var httpResponseMessage = await client.GetAsync(url);

            httpResponseMessage.EnsureSuccessStatusCode();

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var responseContent = await httpResponseMessage.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<ProductDto>(responseContent);
                return View(product);
            }
            TempData["Error"] = "Product doesn't exist.";
            return RedirectToAction("Doesn'tExist", "Product");

        }
    }

}

