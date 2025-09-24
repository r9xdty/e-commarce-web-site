namespace CategoriesWithProducts.UI.Models.DTO
{
    public class CouponDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public bool IsPercent { get; set; }
        public decimal DiscountRate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public ICollection<UserCouponDto> UserCoupons { get; set; }
    }
}
