
using CategoriesWithProducts.Models.Entities;

namespace CategoriesWithProducts.Models.DTO
{
    public class OrderItemDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }

        // API tüketicisi için okunabilir detaylar:
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImageUrl { get; set; }
        public decimal ProductPrice { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public bool IsRefund { get; set; }
        public bool IsUpdated { get; set; }
    }
}
