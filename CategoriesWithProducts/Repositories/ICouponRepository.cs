using CategoriesWithProducts.Models.DTO;
using CategoriesWithProducts.Models.Entities;

namespace CategoriesWithProducts.Repositories
{
    public interface ICouponRepository
    {
        Task<bool> ExistsAsync(string code);
        Task CreateAsync(Coupon coupon);
        Task<Coupon?> GetByCodeAsync(string code);
        Task<bool> HasUserUsedCouponAsync(Guid couponId, Guid userId);
        Task AddUserCouponAsync(UserCoupon userCoupon);
        Task<List<Coupon>> GetCouponsAsync();
        Task<Coupon?> DeleteCouponAsync(Guid id);
        Task<bool> RemoveUserCouponAsync(Guid userId, Guid couponId);
        Task<List<Coupon>> GetUsedCouponsByUserIdAsync(Guid userId);
        Task<List<Coupon>> GetCouponsWithUsersAsync();
    }
}
