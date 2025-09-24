using CategoriesWithProducts.UI.Models.DTO;

namespace CategoriesWithProducts.UI.Models
{
    public class CheckoutViewModel
    {
        public List<CartItemDto> CartItems { get; set; }
        public decimal TotalPrice { get; set; }
        public bool FreeShipping { get; set; }
        public decimal ProductTotalPrice { get; set; }
        public decimal ShippingPrice { get; set; } = 50;

        public CouponDto AppliedCoupon { get; set; }
        public decimal DiscountAmount { get; set; }
    }
}
