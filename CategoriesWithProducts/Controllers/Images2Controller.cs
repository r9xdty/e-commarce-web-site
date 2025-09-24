using System.Net;
using CategoriesWithProducts.API.Repositories;
using CategoriesWithProducts.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CategoriesWithProducts.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Images2Controller : ControllerBase
    {
        private readonly IImageRepository2 imageRepository2;

        public Images2Controller(IImageRepository2 imageRepository2)
        {
            this.imageRepository2 = imageRepository2;
        }

        [HttpPost("cloudinary-image")]
        public async Task<IActionResult> UploadAsync(IFormFile file)
        {
            //call a repository
            var imageUrl = await imageRepository2.UploadAsync(file);

            if (imageUrl == null)
            {
                return Problem("Something went wrong.", null, (int)HttpStatusCode.InternalServerError);
            }
            return new JsonResult(new { link = imageUrl });
        }
    }
}
