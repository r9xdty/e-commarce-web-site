namespace CategoriesWithProducts.UI.Models.DTO
{
    public class AddOrderDto
    {
        public List<AddOrderItemDto> OrderItems { get; set; } = new();
    }
}
