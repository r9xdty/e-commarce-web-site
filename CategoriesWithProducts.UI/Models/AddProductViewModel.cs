using CategoriesWithProducts.UI.Models.DTO;

namespace CategoriesWithProducts.UI.Models
{
    public class AddProductViewModel
    {
        public string Name { get; set; }
        public string PageTitle { get; set; }
        public string Content { get; set; }
        public decimal Price { get; set; }
        public string? ProductImageUrl { get; set; }
        public Guid CategoryId { get; set; }
        public CategoryDto Category { get; set; }
    }
}
