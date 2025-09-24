using CategoriesWithProducts.UI.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace CategoriesWithProducts.UI.ViewComponents
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CategoryMenuViewComponent(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://localhost:7287/api/category");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
                var random = data.ToList();
                return View(random);
            }

            return View(new List<CategoryDto>());
        }
    }
}
