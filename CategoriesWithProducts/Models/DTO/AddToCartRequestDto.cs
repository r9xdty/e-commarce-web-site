namespace CategoriesWithProducts.Models.DTO
{
    public class AddToCartRequestDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
