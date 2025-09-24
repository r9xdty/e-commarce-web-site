using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CategoriesWithProducts.UI.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using CategoriesWithProducts.UI.Models.DTO;
using System.Net.Http;

namespace CategoriesWithProducts.UI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly HttpClient httpClient;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IHttpClientFactory httpClientFactory;

    public HomeController(ILogger<HomeController> logger,
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        this.httpClient = httpClient;
        this.httpContextAccessor = httpContextAccessor;
        this.httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index()
    {
        var categoryResponse = await httpClient.GetAsync("api/category");
        var categoryList = await categoryResponse.Content.ReadFromJsonAsync<List<CategoryDto>>();

        var categoriesWithProducts = new List<CategoryWithProductsViewModel>();

        foreach (var category in categoryList)
        {
            var productResponse = await httpClient.GetAsync($"api/products/{category.Id}/products");

            if (productResponse.IsSuccessStatusCode)
            {
                var products = await productResponse.Content.ReadFromJsonAsync<List<ProductDto>>();

                categoriesWithProducts.Add(new CategoryWithProductsViewModel
                {
                    Category = category,
                    Products = products
                });
            }
        }

        var viewModel = new HomePageViewModel
        {
            CategoriesWithProducts = categoriesWithProducts
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
