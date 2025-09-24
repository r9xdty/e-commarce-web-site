using System.Security.AccessControl;
using CategoriesWithProducts.Models.DTO;
using CategoriesWithProducts.Models.Entities;
using CategoriesWithProducts.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CategoriesWithProducts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository imageRepository;

        public ImagesController(IImageRepository imageRepository)
        {
            this.imageRepository = imageRepository;
        }


        // POST: /api/Images/Upload
        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload([FromForm] ImageUploadRequestDto request)
        {
            ValidateFileUpload(request);

            if (ModelState.IsValid)
            {
                //Convert Dto to Domain model
                var imageDomainModel = new Image
                {
                    File = request.File,
                    FileExtension = Path.GetExtension(request.File.FileName),
                    FileSizeInBytes = request.File.Length,
                    FileName = request.FileName,
                    FileDescription = request.FileDescription,
                };

                //Upload Image Repository
                await imageRepository.Upload(imageDomainModel);

                return Ok(new
                {
                    link = imageDomainModel.FilePath
                });

            }
            return BadRequest(ModelState);
        }

        private void ValidateFileUpload(ImageUploadRequestDto request)
        {
            var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };

            if(!allowedExtensions.Contains(Path.GetExtension(request.File.FileName)))
            {
                ModelState.AddModelError("file", "Unsupported file extension.");
            }

            if (request.File.Length > 10485760)//byte cinsinden 10 MB
            {
                ModelState.AddModelError("file", "File size cannot be more than 10 Mbytes. Please upload smaller file.");
            }
        }
    }
}
