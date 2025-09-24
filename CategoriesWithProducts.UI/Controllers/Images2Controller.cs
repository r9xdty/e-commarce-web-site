using CategoriesWithProducts.UI.Models;
using Microsoft.AspNetCore.Mvc;

namespace CategoriesWithProducts.UI.Controllers
{
    public class Images2Controller : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public Images2Controller(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] UploadImageViewModel model)
        {
            if (model.File == null || model.File.Length == 0)
            {
                ModelState.AddModelError("File", "Please upload a file.");
                return View("Index", model);
            }

            var client = httpClientFactory.CreateClient();
            var url = $"https://localhost:7287/api/Images2/cloudinary-image";

            using var formData = new MultipartFormDataContent();
            formData.Add(new StreamContent(model.File.OpenReadStream()), "file", model.File.FileName);

            var response = await client.PostAsync(url, formData);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadFromJsonAsync<UploadResponse>();
                ViewBag.ImageUrl = responseContent?.Link;
                return View("Index");
            }

            ModelState.AddModelError(string.Empty, "An error occurred while uploading the file.");
            return View("Index", model);
        }
    }
}

