using System.Security.Claims;
using System.Text.Json;
using AutoMapper;
using CategoriesWithProducts.Models.DTO;
using CategoriesWithProducts.Models.Entities;
using CategoriesWithProducts.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace CategoriesWithProducts.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CouponsController : ControllerBase
    {
        private readonly ICouponRepository couponRepository;
        private readonly IMapper mapper;
        private readonly ILogger<CategoryController> logger;

        public CouponsController(ICouponRepository couponRepository, IMapper mapper, ILogger<CategoryController> logger)
        {
            this.couponRepository = couponRepository;
            this.mapper = mapper;
            this.logger = logger;
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponDto couponDto)
        {
            if (await couponRepository.ExistsAsync(couponDto.Code))
                return BadRequest("This coupon code already exists.");

            var coupon = new Coupon
            {
                Id = Guid.NewGuid(),
                Code = couponDto.Code,
                IsPercent = couponDto.IsPercent,
                DiscountRate = couponDto.DiscountRate,
                ExpirationDate = couponDto.ExpirationDate
            };

            await couponRepository.CreateAsync(coupon);

            return Ok(mapper.Map<CouponDto>(coupon));
        }

        [Authorize]
        [HttpPost("apply")]
        public async Task<IActionResult> ApplyCoupon([FromBody] ApplyCouponDto applyCouponDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.Email);

            var coupon = await couponRepository.GetByCodeAsync(applyCouponDto.Code);
            if (coupon == null)
                return NotFound("Coupon not found.");

            if (coupon.ExpirationDate < DateTime.UtcNow)
                return BadRequest("Coupon has expired.");

            var used = await couponRepository.HasUserUsedCouponAsync(coupon.Id, Guid.Parse(userId));
            if (used)
                return BadRequest("You have already used this coupon.");

            var userCoupon = new UserCoupon
            {

                Id = Guid.NewGuid(),
                UserId = Guid.Parse(userId),
                UserName = userName,
                CouponId = coupon.Id,
                CouponCode = coupon.Code,
                UsedAt = DateTime.UtcNow,
            };

            await couponRepository.AddUserCouponAsync(userCoupon);

            var couponApplyed = mapper.Map<Coupon>(coupon);

            return Ok(couponApplyed);

        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetCoupons()
        {
            var couponsDomain = await couponRepository.GetCouponsAsync();

            logger.LogInformation($"Finished GetAllCategories request with data: {JsonSerializer.Serialize(couponsDomain)}");

            return Ok(mapper.Map<List<CouponDto>>(couponsDomain));
        }

        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCoupon([FromRoute] Guid id)
        {
            var couponsDomainModel = await couponRepository.DeleteCouponAsync(id);

            if (couponsDomainModel == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<CouponDto>(couponsDomainModel));
        }

        [Authorize]
        [HttpGet("used")]
        public async Task<IActionResult> GetUsedCouponsById()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid userId))
                return Unauthorized();

            var usedCoupons = await couponRepository.GetUsedCouponsByUserIdAsync(userId);

            var result = usedCoupons.Select(c => new
            {
                c.Code,
                c.DiscountRate,
                c.IsPercent,
                c.ExpirationDate
            });

            return Ok(result);
        }

        [Authorize]
        [HttpDelete("unapply")]
        public async Task<IActionResult> UnapplyCoupon([FromQuery] string code)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid userId))
                return Unauthorized();

            var coupon = await couponRepository.GetByCodeAsync(code);
            if (coupon == null)
                return NotFound("Coupon not found.");

            var removed = await couponRepository.RemoveUserCouponAsync(userId, coupon.Id);
            if (!removed)
                return NotFound("Coupon usage not found for this user.");

            return Ok("Coupon removed successfully.");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("used-by-users")]
        public async Task<IActionResult> GetUsedCouponsByUsers()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.Email);

            var coupons = await couponRepository.GetCouponsWithUsersAsync();

            var result = coupons.SelectMany(c => c.UserCoupons.Select(uc => new
            {
                CouponId = c.Id,
                CouponCode = c.Code,
                UserId = userId,
                UserName = userName,
                UsedAt = uc.UsedAt,
                IsDeleted = uc.IsDeleted,
                User = new UserDto
                {
                    Id = userId,
                    UserName = userName
                }
            }));

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("unapply-by-admin")]
        public async Task<IActionResult> UnapplyUserCoupons([FromQuery] Guid userId, [FromQuery] string code)
        {
            var coupon = await couponRepository.GetByCodeAsync(code);
            if (coupon == null)
                return NotFound("Coupon not found.");

            var hasUsed = await couponRepository.HasUserUsedCouponAsync(coupon.Id, userId);
            if (!hasUsed)
                return BadRequest("User has not used this coupon.");

            var removed = await couponRepository.RemoveUserCouponAsync(userId, coupon.Id);
            if (!removed)
                return StatusCode(500, "Could not unapply the coupon.");

            return Ok("Coupon has been unapplied for the user.");
        }
    }
}

