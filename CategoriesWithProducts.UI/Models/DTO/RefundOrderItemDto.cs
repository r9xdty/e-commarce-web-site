namespace CategoriesWithProducts.UI.Models.DTO
{
    public class RefundOrderItemDto
    {
        public Guid OrderId { get; set; }
        public Guid OrderItemId { get; set; }
        public int RefundQuantity { get; set; }
    }
}
