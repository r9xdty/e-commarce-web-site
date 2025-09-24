using System.ComponentModel.DataAnnotations;

namespace CategoriesWithProducts.UI.Models.DTO
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "Email zorunludur")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Geçerli bir email giriniz")]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string[] Roles { get; set; }
    }
}
