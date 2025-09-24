namespace CategoriesWithProducts.UI.Models.DTO
{
    public class AddOrderItemDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
