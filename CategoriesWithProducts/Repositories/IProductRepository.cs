using CategoriesWithProducts.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CategoriesWithProducts.Repositories
{
    public interface IProductRepository
    {
        Task<Product> CreateProductAsync(Product product, Guid categoryId);
        Task<List<Product>> GetAllProductsAsync(string? filterOn = null, string? filterQuery = null,
            decimal? minValue = null, decimal? maxValue = null, string? sortBy = null, bool isAscending = true,
            int pageNumber = 1 , int pageSize = 1000);
        Task<Product?> GetProductByIdAsync(Guid id);
        Task<Product?> UpdateProductAsync(Guid id, Product product);
        Task<Product?> DeleteProductAsync(Guid id);
        Task<List<Product>> GetProductsByCategoryAsync(Guid categoryId, string? filterQuery = null,
            decimal? minValue = null, decimal? maxValue = null, string? sortBy = null, bool isAscending = true,
            int pageNumber = 1, int pageSize = 1000);
        Task<int> GetFilteredProductsCountAsync(string? filterQuery = null,
            string? sortBy = null,
            bool isAscending = true,
            decimal? minValue = null,
            decimal? maxValue = null,
            Guid? categoryId = null);
    }
}
