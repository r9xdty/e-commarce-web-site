using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CategoriesWithProducts.Models.DTO
{
    public class AddCategoryDto
    {
        [Required]
        [DefaultValue("")]
        [MinLength(2, ErrorMessage = "Category name needs at least 2 characters")]
        public string Name { get; set; }

    }
}
