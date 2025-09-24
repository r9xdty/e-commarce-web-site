// Helpers/CookieHelper.cs
using CategoriesWithProducts.UI.Models.DTO;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace CategoriesWithProducts.UI.Helpers
{
    public static class CookieHelper
    {

        public static CartDto GetGuestCart(HttpContext context)
        {
            if (context.Request.Cookies.TryGetValue("cart_guest", out var json))
            {
                return JsonSerializer.Deserialize<CartDto>(json) ?? new CartDto();
            }
            return new CartDto();
        }

        public static void ClearGuestCart(HttpContext context)
        {
            context.Response.Cookies.Delete("cart_guest");
        }

        public static void MergeGuestCartToUserCart(HttpContext context, string userId)
        {
            var guestCart = GetGuestCart(context);
            var userCartKey = $"cart_{userId}";
            CartDto userCart = new();

            
            if (context.Request.Cookies.TryGetValue(userCartKey, out var userCartJson))
            {
                userCart = JsonSerializer.Deserialize<CartDto>(userCartJson) ?? new CartDto();
            }

            foreach (var guestItem in guestCart.Items)
            {
                var existingItem = userCart.Items.FirstOrDefault(x => x.ProductId == guestItem.ProductId);
                if (existingItem != null)
                {
                    existingItem.Quantity += guestItem.Quantity;
                }
                else
                {
                    userCart.Items.Add(guestItem);
                }
            }

            var mergedJson = JsonSerializer.Serialize(userCart);
            context.Response.Cookies.Append(userCartKey, mergedJson, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(90)
            });

            ClearGuestCart(context);
        }
        public static string? GetUserId(HttpContext context)
        {
            return context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
        public static string GetCartKey(HttpContext context)
        {
            {
                var userId = GetUserId(context);
                return string.IsNullOrEmpty(userId) ? "cart_guest" : $"cart_{userId}";
            }
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
                Secure = false,
                SameSite = SameSiteMode.Lax,
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
