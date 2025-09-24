using CategoriesWithProducts.Models.Entities;

namespace CategoriesWithProducts.Repositories
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllCategoriesWithTotalPriceAsync();

        Task<List<Category>> GetAllCategoriesAsync();

        Task<Category?> GetCategoryByIdAsync(Guid id);

        Task<Category> AddCategoryAsync(Category category);

        Task<Category?> UpdateCategoryAsync(Guid id, Category category);

        Task<Category?> DeleteCategoryAsync(Guid id);

    }
}
