using CategoriesWithProducts.UI.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using CategoriesWithProducts.UI.Helpers;
using System.Security.Claims;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CategoriesWithProducts.UI.Models;
using System.Runtime.InteropServices;

namespace CategoriesWithProducts.UI.Controllers
{
    public class CartController : Controller
    {
        public IActionResult GetCarts()
        {

            var cart = CookieHelper.GetCart(HttpContext);
            ViewBag.CartCount = cart?.Items?.Count ?? 0;
            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(CartItemDto newItem)
        {
            var cart = CookieHelper.GetCart(HttpContext);

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == newItem.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += newItem.Quantity;
            }
            else
            {
                cart.Items.Add(newItem);
            }

            CookieHelper.SetCart(HttpContext, cart);
            TempData["SuccessMessage"] = "Ürün sepetinize eklendi.";
            return RedirectToAction("GetCarts");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(Guid productId)
        {
            var cart = CookieHelper.GetCart(HttpContext);

            cart.Items = cart.Items.Where(item => item.ProductId != productId).ToList();

            CookieHelper.SetCart(HttpContext, cart);
            TempData["SuccessMessage"] = "Ürün sepetten çıkarıldı.";
            return RedirectToAction("GetCarts");
        }

        [HttpPost]
        public IActionResult ClearCart()
        {
            CookieHelper.ClearCart(HttpContext);
            TempData["SuccessMessage"] = "Sepetiniz boşaltıldı.";
            return RedirectToAction("GetCarts");
        }

        [HttpPost]
        public IActionResult UpdateQuantity(Guid productId, string operation)
        {
            var cart = CookieHelper.GetCart(HttpContext);
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);

            if (item != null)
            {
                if (operation == "increase") item.Quantity++;
                else if (operation == "decrease" && item.Quantity > 1) item.Quantity--;
            }

            CookieHelper.SetCart(HttpContext, cart);
            return RedirectToAction("GetCarts");
        }
    }
}
