using CategoriesWithProducts.Models.DTO;
using CategoriesWithProducts.Models.Entities;

namespace CategoriesWithProducts.Repositories
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllOrdersAsync();
        Task<List<Order>> GetOrdersByUserIdAsync(Guid userId);

        Task<Order?> GetOrderByIdAsync(Guid id);

        Task<Order> AddOrderAsync(Order order);
        Task<bool> RefundOrderItemAsync(RefundOrderItemDto refundDto);
    }
}
