using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CategoriesWithProducts.Models.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public decimal TotalPrice { get; set; } = 0;
        public bool IsDeleted { get; set; }
    }
}
