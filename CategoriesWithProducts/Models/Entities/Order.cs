namespace CategoriesWithProducts.Models.Entities
{
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }  // Token'dan alınacak
        public string? UserName { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
