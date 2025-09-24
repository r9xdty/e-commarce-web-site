namespace CategoriesWithProducts.Models.DTO
{
    public class CreateCouponDto
    {
        public string Code { get; set; } = string.Empty;
        public bool IsPercent { get; set; }
        public decimal DiscountRate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
