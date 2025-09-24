using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CategoriesWithProducts.Models.DTO
{
    public class UpdateProductDto
    {
        [Required]
        [DefaultValue("")]
        [MinLength(2, ErrorMessage = "Product name needs at least 2 characters")]
        public string Name { get; set; }

        public string PageTitle { get; set; }
        public string Content { get; set; }
        [Required]
        [DefaultValue("")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than '0'.")]
        public decimal Price { get; set; }
        public string? ProductImageUrl { get; set; }

        [Required]
        public Guid CategoryId { get; set; }
    }
}
