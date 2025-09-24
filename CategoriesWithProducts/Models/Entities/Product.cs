using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CategoriesWithProducts.Models.Entities
{
    public class Product
    {

        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string PageTitle { get; set; }
        public string Content { get; set; }
        public decimal Price { get; set; }
        public string? ProductImageUrl { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
        public bool IsDeleted { get; set; }
    }

}

