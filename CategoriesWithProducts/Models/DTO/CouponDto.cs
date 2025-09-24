using CategoriesWithProducts.Models.Entities;

namespace CategoriesWithProducts.Models.DTO
{
    public class CouponDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public bool IsPercent { get; set; }
        public decimal DiscountRate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public ICollection<UserCoupon> UserCoupons { get; set; }
        public bool IsDeleted { get; set; }
    }
}
