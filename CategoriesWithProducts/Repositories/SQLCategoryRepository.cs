using CategoriesWithProducts.Data;
using CategoriesWithProducts.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CategoriesWithProducts.Repositories
{
    public class SQLCategoryRepository : ICategoryRepository
    {
        private readonly ListDBContext dBContext;

        public SQLCategoryRepository(ListDBContext dBContext)
        {
            this.dBContext = dBContext;
        }

        public async Task<Category> AddCategoryAsync(Category category)
        {
            await dBContext.Categories.AddAsync(category);
            await dBContext.SaveChangesAsync();
            return category;
        }

        public async Task<Category?> DeleteCategoryAsync(Guid id)
        {
            var existingCategory = await dBContext.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (existingCategory == null)
            {
                return null;
            }

            existingCategory.IsDeleted = true;
            await dBContext.SaveChangesAsync();
            return existingCategory;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            var categories = await dBContext.Categories
                .Where(c => !c.IsDeleted)
                .Select(c => new Category
                {
                    Id = c.Id,
                    Name = c.Name,
                    
                    TotalPrice = dBContext.Products
                        .Where(p => p.CategoryId == c.Id && !p.IsDeleted)
                        .Sum(p => (decimal?)p.Price) ?? 0
                })
                .ToListAsync();
            await dBContext.SaveChangesAsync();
            return categories;
        }

        public async Task<List<Category>> GetAllCategoriesWithTotalPriceAsync()
        {
            {
                var categories = await dBContext.Categories.ToListAsync();

                foreach (var category in categories)
                {
                    category.TotalPrice = await dBContext.Products
                        .Where(p => p.CategoryId == category.Id)
                        .SumAsync(p => p.Price);
                }

                return categories.Select(c => new Category
                {
                    Id = c.Id,
                    Name = c.Name,
                    TotalPrice = c.TotalPrice
                }).ToList();
            }
        }

        public async Task<Category?> GetCategoryByIdAsync(Guid id)
        {
            return await dBContext.Categories.FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<Category?> UpdateCategoryAsync(Guid id, Category category)
        {
            var existingCategory = await dBContext.Categories.FirstOrDefaultAsync(x =>x.Id == id);
            if (existingCategory == null)
            {
                return null;
            }

            existingCategory.Name = category.Name;
            existingCategory.TotalPrice = category.TotalPrice;

            await dBContext.SaveChangesAsync();
            return existingCategory;
        }
    }
}
