/*using System.Security.Claims;
using System.Text.Json;
using CategoriesWithProducts.Models.DTO;

namespace CategoriesWithProducts.Helpers
{
    public static class CookieHelper
    {
        public static string GetCartKey(HttpContext context)
        {
            var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return string.IsNullOrEmpty(userId) ? "cart" : $"cart_{userId}";
        }

        public static CartDto GetCart(HttpContext context)
        {
            var key = GetCartKey(context);
            if (context.Request.Cookies.TryGetValue(key, out var json))
            {
                return JsonSerializer.Deserialize<CartDto>(json) ?? new CartDto();
            }
            return new CartDto();
        }

        public static void SetCart(HttpContext context, CartDto cart)
        {
            var key = GetCartKey(context);
            var json = JsonSerializer.Serialize(cart);
            context.Response.Cookies.Append(key, json, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(90)
            });
        }

        public static void ClearCart(HttpContext context)
        {
            var key = GetCartKey(context);
            context.Response.Cookies.Delete(key);
        }
    }
}
*/