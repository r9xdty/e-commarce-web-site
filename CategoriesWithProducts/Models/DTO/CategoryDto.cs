using CategoriesWithProducts.Models.Entities;

namespace CategoriesWithProducts.Models.DTO
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public decimal TotalPrice { get; set; } = 0;
        public bool IsDeleted { get; set; }
    }
}
