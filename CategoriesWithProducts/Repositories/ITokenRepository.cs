using CategoriesWithProducts.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace CategoriesWithProducts.Repositories
{
    public interface ITokenRepository
    {
        string CreateJWTToken(ApplicationUser user, List<string> roles);
    }
}
