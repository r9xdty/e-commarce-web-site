using CategoriesWithProducts.Data;
using CategoriesWithProducts.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CategoriesWithProducts.Repositories
{
    public class SQLProductRepository : IProductRepository
    {
        private readonly ListDBContext dbContext;

        public SQLProductRepository(ListDBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<int> GetFilteredProductsCountAsync(
            string? filterQuery = null,
            string? sortBy = null,
            bool isAscending = true,
            decimal? minValue = null,
            decimal? maxValue = null,
            Guid? categoryId = null
)
        {
            var query = dbContext.Products.AsQueryable();

            if (!string.IsNullOrEmpty(filterQuery))
            {
                query = query.Where(p =>
                    p.Name.Contains(filterQuery) ||
                    p.Content.Contains(filterQuery));
            }

            if (minValue.HasValue)
            {
                query = query.Where(p => p.Price >= minValue.Value);
            }

            if (maxValue.HasValue)
            {
                query = query.Where(p => p.Price <= maxValue.Value);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            return await query.CountAsync();
        }
        public async Task<Product> CreateProductAsync(Product product, Guid categoryId)
        {
            product.CategoryId = categoryId;
            await dbContext.Products.AddAsync(product);
            await dbContext.SaveChangesAsync();
            return product;
        }


        public async Task<Product?> DeleteProductAsync(Guid id)
        {
            var existingProduct = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (existingProduct == null)
            {
                return null;
            }

            existingProduct.IsDeleted = true;
            await dbContext.SaveChangesAsync();
            return existingProduct;
        }

        public async Task<List<Product>> GetAllProductsAsync(string filterOn = "Name", string? filterQuery = null,
            decimal? minValue = null, decimal? maxValue = null, string? sortBy = null, bool isAscending = true,
            int pageNumber = 1, int pageSize = 10)
        {
            var products = dbContext.Products.Include(x => x.Category).Where(c => !c.IsDeleted).AsQueryable();


            //Filtering
            if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
            {
                if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    products = products.Where(x => x.Name.Contains(filterQuery));
                }
            }
            if (minValue.HasValue || maxValue.HasValue)
            {
                products = products.Where(p => (!minValue.HasValue || p.Price >= minValue) && (!maxValue.HasValue || p.Price <= maxValue));
            }

            //Sorting
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    products = isAscending ? products.OrderBy(x => x.Name) : products.OrderByDescending(x => x.Name);
                }
                else if (sortBy.Equals("Price", StringComparison.OrdinalIgnoreCase))
                {
                    products = isAscending ? products.OrderBy(x => x.Price) : products.OrderByDescending(x => x.Price);
                }
            }

            //Pagination
            var skipResults = (pageNumber - 1) * pageSize;

            return await products.Skip(skipResults).Take(pageSize).ToListAsync();
            
        }

        public async Task<List<Product>> GetPriceForCategoryAsync(Guid categoryId)
        {
            var products = await dbContext.Products
                .Where(x => x.CategoryId == categoryId)
                .ToListAsync();

            var totalPrice = products.Sum(x => x.Price);

            var category = await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);
            if (category != null)
            {
                category.TotalPrice = totalPrice;
                await dbContext.SaveChangesAsync(); // Değişiklikleri veritabanına kaydet
            }

            return products;
        }

        public async Task<Product?> GetProductByIdAsync(Guid id)
        {
            return await dbContext.Products.Include("Category").FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Product>> GetProductsByCategoryAsync(Guid categoryId, string? filterQuery = null,
            decimal? minValue = null, decimal? maxValue = null, string? sortBy = null, bool isAscending = true,
            int pageNumber = 1, int pageSize = 1000)
        {
            var products = dbContext.Products.Include("Category").Where(c => !c.IsDeleted).AsQueryable();

            products = products.Where(x => x.CategoryId == categoryId);

            //Filtering
            if (!string.IsNullOrWhiteSpace(filterQuery))
            {
                products = products.Where(x => x.Name.Contains(filterQuery));

            }
            if (minValue.HasValue || maxValue.HasValue)
            {
                products = products.Where(p => (!minValue.HasValue || p.Price >= minValue) && (!maxValue.HasValue || p.Price <= maxValue));
            }

            //Sorting
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    products = isAscending ? products.OrderBy(x => x.Name) : products.OrderByDescending(x => x.Name);
                }
                else if (sortBy.Equals("Price", StringComparison.OrdinalIgnoreCase))
                {
                    products = isAscending ? products.OrderBy(x => x.Price) : products.OrderByDescending(x => x.Price);
                }
            }

            //Pagination
            var skipResults = (pageNumber - 1) * pageSize;

            return await products.Skip(skipResults).Take(pageSize).ToListAsync();

        }


        public async Task<Product?> UpdateProductAsync(Guid id, Product product)
        {
            var existingProduct = await dbContext.Products.FirstOrDefaultAsync(x => x.Id == id);

            if (existingProduct == null)
            {
                return null;
            }

            existingProduct.Name = product.Name;
            existingProduct.Content = product.Content;
            existingProduct.PageTitle = product.PageTitle;
            existingProduct.Price = product.Price;
            existingProduct.ProductImageUrl = product.ProductImageUrl;
            existingProduct.CategoryId = product.CategoryId;

            await dbContext.SaveChangesAsync();

            return existingProduct;
        }

    }
}
