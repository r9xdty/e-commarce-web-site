using CategoriesWithProducts.Models.Entities;

namespace CategoriesWithProducts.Repositories
{
    public interface IImageRepository
    {
        Task<Image> Upload(Image image);
    }
}
