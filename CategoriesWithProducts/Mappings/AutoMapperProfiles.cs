using AutoMapper;
using CategoriesWithProducts.Models.DTO;
using CategoriesWithProducts.Models.Entities;

namespace CategoriesWithProducts.Mappings
{
    public class AutoMapperProfiles : Profile
    {

        public AutoMapperProfiles()
        {
            // Category
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<AddCategoryDto, Category>().ReverseMap();
            CreateMap<UpdateCategoryDto, Category>().ReverseMap();

            // Product
            CreateMap<AddProductDto, Product>().ReverseMap();
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<UpdateProductDto, Product>().ReverseMap();

            // Coupon
            CreateMap<Coupon, CouponDto>()
                .ForMember(dest => dest.UserCoupons, opt => opt.Ignore());
            CreateMap<CouponDto, Coupon>();
            CreateMap<UserCoupon, UserCouponDto>().ReverseMap();
            CreateMap<CreateCouponDto, Coupon>().ReverseMap();
            CreateMap<ApplyCouponDto, Coupon>().ReverseMap();

            // Order
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<AddOrderDto, Order>().ReverseMap();

            // OrderItem
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : ""))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product != null ? src.Product.Price : 0))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductImageUrl : null))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Product != null && src.Product.Category != null ? src.Product.Category.Name : ""));
                

            CreateMap<AddOrderItemDto, OrderItem>();
            CreateMap<OrderItem, OrderItemDto>().ReverseMap();

        }
    }
}
