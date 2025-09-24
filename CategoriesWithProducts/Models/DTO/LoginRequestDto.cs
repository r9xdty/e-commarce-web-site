using System.ComponentModel.DataAnnotations;

namespace CategoriesWithProducts.Models.DTO
{
    public class LoginRequestDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string? ReturnUrl { get; set; }
        public bool IsDeleted { get; set; }

    }
}
