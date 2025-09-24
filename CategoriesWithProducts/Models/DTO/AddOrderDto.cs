using CategoriesWithProducts.Models.Entities;

namespace CategoriesWithProducts.Models.DTO
{
    public class AddOrderDto
    {
        public List<AddOrderItemDto> OrderItems { get; set; } = new();
    }
}
