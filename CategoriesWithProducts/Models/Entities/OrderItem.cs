namespace CategoriesWithProducts.Models.Entities
{
    public class OrderItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProductId { get; set; }
        public string? ProductImageUrl { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }

        // Foreign key to Order
        public Guid OrderId { get; set; }
        public Order Order { get; set; }

        public bool IsRefund { get; set; } = false;
        public bool IsUpdated { get; set; } = false;
    }
}
