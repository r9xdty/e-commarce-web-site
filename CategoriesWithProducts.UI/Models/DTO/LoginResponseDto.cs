using Newtonsoft.Json;

namespace CategoriesWithProducts.UI.Models.DTO
{
    public class LoginResponseDto
    {
        public string Id { get; set; }
        public string JwtToken { get; set; }
    }
}
