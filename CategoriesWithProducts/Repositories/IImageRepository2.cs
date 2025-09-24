namespace CategoriesWithProducts.API.Repositories
{
    public interface IImageRepository2
    {
        Task<string> UploadAsync(IFormFile file);
    }
}
