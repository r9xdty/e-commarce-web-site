using CategoriesWithProducts.UI.Models.DTO;

namespace CategoriesWithProducts.UI.Models
{
    public class HomeViewModel
    {
        public IEnumerable<CategoryDto> Categories { get; set; }

        public IEnumerable<ProductDto> Products { get; set; }
    }
}
