using System.ComponentModel.DataAnnotations;

namespace CategoriesWithProducts.UI.Models.DTO
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Email zorunludur")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Geçerli bir email giriniz")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Şifre zorunludur")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "Şifre en az 6 karakter olmalı.", MinimumLength = 6)]
        public string Password { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
