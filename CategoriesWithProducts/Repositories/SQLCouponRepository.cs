using System;
using System.Security.Claims;
using CategoriesWithProducts.Data;
using CategoriesWithProducts.Models.DTO;
using CategoriesWithProducts.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CategoriesWithProducts.Repositories
{
    public class SQLCouponRepository : ICouponRepository
    {
        private readonly ListDBContext dbContext;

        public SQLCouponRepository(ListDBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> ExistsAsync(string code)
        {
            return await dbContext.Coupons.AnyAsync(c => c.Code == code);
        }

        public async Task CreateAsync(Coupon coupon)
        {
            await dbContext.Coupons.AddAsync(coupon);
            await dbContext.SaveChangesAsync();
        }

        public async Task<Coupon?> GetByCodeAsync(string code)
        {
            return await dbContext.Coupons.Include(c => c.UserCoupons).FirstOrDefaultAsync(c => c.Code == code);
        }

        public async Task<bool> HasUserUsedCouponAsync(Guid couponId, Guid userId)
        {
            return await dbContext.UserCoupons.AnyAsync(uc => uc.CouponId == couponId && uc.UserId == userId);
        }

        public async Task AddUserCouponAsync(UserCoupon userCoupon)
        {
            await dbContext.UserCoupons.AddAsync(userCoupon);
            await dbContext.SaveChangesAsync();
        }

        public async Task<List<Coupon>> GetCouponsAsync()
        {
            return await dbContext.Coupons.Where(c => !c.IsDeleted).ToListAsync();
        }

        public async Task<Coupon?> DeleteCouponAsync(Guid id)
        {
            var existingCoupon = await dbContext.Coupons.FirstOrDefaultAsync(x => x.Id == id);

            if (existingCoupon == null)
            {
                return null;
            }
            existingCoupon.IsDeleted = true;
            await dbContext.SaveChangesAsync();
            return existingCoupon;
        }

        public async Task<bool> RemoveUserCouponAsync(Guid userId, Guid couponId)
        {
            var userCoupon = await dbContext.UserCoupons
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CouponId == couponId);

            if (userCoupon == null)
                return false;

            dbContext.UserCoupons.Remove(userCoupon);
            await dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<List<Coupon>> GetUsedCouponsByUserIdAsync(Guid userId)
        {
            var couponIds = await dbContext.UserCoupons
                .Where(uc => uc.UserId == userId)
                .Select(uc => uc.CouponId)
                .ToListAsync();

            return await dbContext.Coupons
                .Where(c => couponIds.Contains(c.Id))
                .ToListAsync();
        }

        public async Task<List<Coupon>> GetCouponsWithUsersAsync()
        {
            return await dbContext.Coupons
                .Where(c => !c.IsDeleted)
                .Include(c => c.UserCoupons)
                .ToListAsync();
        }
    }
}
