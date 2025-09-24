using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace CategoriesWithProducts.Models.DTO
{
    public class UpdateCategoryDto
    {
        [Required]
        [DefaultValue("")]
        [MinLength(2, ErrorMessage = "Category name needs at least 2 characters")]
        public string Name { get; set; }
    }
}
