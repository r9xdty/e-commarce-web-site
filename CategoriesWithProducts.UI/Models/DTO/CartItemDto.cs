namespace CategoriesWithProducts.UI.Models.DTO
{
    public class CartItemDto
    {
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductImageUrl { get; set; }
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
