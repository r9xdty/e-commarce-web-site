using System.Text;
using CategoriesWithProducts.UI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CategoriesWithProducts.UI.Controllers
{
    public class ImagesController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;

        public ImagesController(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        [HttpPost("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile File, string FileName, ImageUploadRequestDto imageUploadRequestDto)
        {
            if (File == null || File.Length == 0)
            {
                return Json(new { success = false, message = "Dosya geçersiz." });
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await File.CopyToAsync(stream);
            }

            // Uygulama kökünden itibaren erişilebilir URL oluştur
            var imageUrl = $"/uploads/{uniqueFileName}";

            return Json(new { success = true, imageUrl });
        }
    }
}
