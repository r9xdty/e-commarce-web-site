using Microsoft.AspNetCore.Identity;

namespace CategoriesWithProducts.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsDeleted { get; set; } = false;
    }
}
