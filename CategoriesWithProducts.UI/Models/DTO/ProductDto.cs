namespace CategoriesWithProducts.UI.Models.DTO
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string PageTitle { get; set; }
        public string Content { get; set; }
        public decimal Price { get; set; }
        public string ProductImageUrl { get; set; }
        public Guid CategoryId { get; set; }
        public CategoryDto Category { get; set; }
    }
}
