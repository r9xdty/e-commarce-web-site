using CategoriesWithProducts.Data;
using CategoriesWithProducts.Models.Entities;

namespace CategoriesWithProducts.Repositories
{
    public class LocalImageRepository : IImageRepository
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ListDBContext listDBContext;

        public LocalImageRepository(IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor, ListDBContext listDBContext)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
            this.listDBContext = listDBContext;
        }
        public async Task<Image> Upload(Image image)
        {
            var localFilePath = Path.Combine(webHostEnvironment.WebRootPath,
                "Images", $"{image.FileName}{image.FileExtension}");

            //Upload Image to Local Path
            using var stream = new FileStream(localFilePath, FileMode.Create);
            await image.File.CopyToAsync(stream);

            // https://localhost:????/images/image.jpg buradan çekicek fotoyu

            var urlFilePath = $"{httpContextAccessor.HttpContext.Request.Scheme}:" +
                $"//{httpContextAccessor.HttpContext.Request.Host}/Images/{image.FileName}{image.FileExtension}";

            image.FilePath = urlFilePath;
            
            //Add Image to the Images Table
            await listDBContext.Images.AddAsync(image);
            await listDBContext.SaveChangesAsync();

            return image;

        }
    }
}
