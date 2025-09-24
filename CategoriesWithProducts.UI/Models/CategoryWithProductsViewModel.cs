using CategoriesWithProducts.UI.Models.DTO;

namespace CategoriesWithProducts.UI.Models
{
    public class CategoryWithProductsViewModel
    {
        public CategoryDto Category { get; set; }
        public List<ProductDto> Products { get; set; }
    }
}
