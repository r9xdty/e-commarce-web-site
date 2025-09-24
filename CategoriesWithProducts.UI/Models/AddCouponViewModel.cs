namespace CategoriesWithProducts.UI.Models
{
    public class AddCouponViewModel
    {
        public string Code { get; set; }
        public bool IsPercent { get; set; }
        public decimal DiscountRate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
