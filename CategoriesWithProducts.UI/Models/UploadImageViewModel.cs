using System.ComponentModel.DataAnnotations;

namespace CategoriesWithProducts.UI.Models
{
    public class UploadImageViewModel
    {
        [Required(ErrorMessage = "Please upload a file.")]
        public IFormFile File { get; set; }
    }
}
