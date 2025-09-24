namespace CategoriesWithProducts.UI.Models.DTO
{
    public class UserCouponDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid CouponId { get; set; }
        public string CouponCode { get; set; }
        public DateTime UsedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
