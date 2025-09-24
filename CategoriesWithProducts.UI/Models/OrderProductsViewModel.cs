using CategoriesWithProducts.UI.Models.DTO;

namespace CategoriesWithProducts.UI.Models
{
    public class OrderItemViewModel
    {
        public OrderItemDto Item { get; set; }
        public string ProductImageUrl { get; set; }
    }

    public class OrderProductsViewModel
    {
        public DateTime CreatedAt { get; set; }
        public int OrderId { get; set; }
        public List<OrderItemViewModel> OrderItems { get; set; }
    }
}
