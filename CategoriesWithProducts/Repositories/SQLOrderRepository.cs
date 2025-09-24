using CategoriesWithProducts.Data;
using CategoriesWithProducts.Models.DTO;
using CategoriesWithProducts.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CategoriesWithProducts.Repositories
{
    public class SQLOrderRepository : IOrderRepository
    {
        private readonly ListDBContext dbContext;

        public SQLOrderRepository(ListDBContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<Order> AddOrderAsync(Order order)
        {
            await dbContext.Orders.AddAsync(order);
            await dbContext.SaveChangesAsync();
            return order;
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await dbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ThenInclude(p => p.Category)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(Guid id)
        {
            return await dbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task<List<Order>> GetOrdersByUserIdAsync(Guid userId)
        {
            return await dbContext.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.Category)
                .Where(o => o.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> RefundOrderItemAsync(RefundOrderItemDto dto)
        {
            var order = await dbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == dto.OrderId);

            if (order == null) return false;

            var item = order.OrderItems.FirstOrDefault(oi => oi.Id == dto.OrderItemId && !oi.IsRefund);

            if (item == null || dto.RefundQuantity <= 0 || dto.RefundQuantity > item.Quantity)
                return false;

            item.Quantity -= dto.RefundQuantity;
            item.IsUpdated = true;

            var refundItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductId = item.ProductId,
                Quantity = dto.RefundQuantity,
                IsRefund = true,
                IsUpdated = false,
            };

            await dbContext.OrderItems.AddAsync(refundItem);
            await dbContext.SaveChangesAsync();

            return true;
        }
    }
}
