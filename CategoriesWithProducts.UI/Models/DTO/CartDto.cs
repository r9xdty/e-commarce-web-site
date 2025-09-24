namespace CategoriesWithProducts.UI.Models.DTO
{
    public class CartDto
    {
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();

        public decimal TotalPrice => Items.Sum(item => item.ProductPrice * item.Quantity);
    }
}
