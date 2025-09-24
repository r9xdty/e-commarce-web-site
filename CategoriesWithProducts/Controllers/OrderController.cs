using System.Security.Claims;
using System.Text.Json;
using AutoMapper;
using Azure.Identity;
using CategoriesWithProducts.CustomActionFilters;
using CategoriesWithProducts.Data;
using CategoriesWithProducts.Models.DTO;
using CategoriesWithProducts.Models.Entities;
using CategoriesWithProducts.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CategoriesWithProducts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class OrderController : ControllerBase
    {
        private readonly ListDBContext dbContext;
        private readonly IOrderRepository orderRepository;
        private readonly IMapper mapper;
        private readonly ILogger<CategoryController> logger;

        public OrderController(ListDBContext dbContext, IOrderRepository orderRepository, IMapper mapper,
            ILogger<CategoryController> logger)
        {
            this.dbContext = dbContext;
            this.orderRepository = orderRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var ordersDomain = await orderRepository.GetAllOrdersAsync();
            var orderDtos = mapper.Map<List<OrderDto>>(ordersDomain);
            return Ok(orderDtos);
        }

        [HttpGet]
        [Route("{id:guid}")]

        public async Task<IActionResult> GetOrderById([FromRoute] Guid id)
        {
            var ordersDomain = await orderRepository.GetOrderByIdAsync(id); //Find sadece id için kullanılabilir.

            if (ordersDomain == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<OrderDto>(ordersDomain));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddOrder([FromBody] AddOrderDto dto)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.Email);
            if (!Guid.TryParse(userIdString, out var userId))
                return Unauthorized();

            var order = new Order
            {
                UserId = userId,
                UserName = userName,
                CreatedAt = DateTime.UtcNow,
                OrderItems = dto.OrderItems.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                }).ToList()
            };

            var createdOrder = await orderRepository.AddOrderAsync(order);

            var orderDto = mapper.Map<OrderDto>(createdOrder);
            return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.Id }, orderDto);
        }

        [HttpGet("user")]
        [Authorize]
        public async Task<IActionResult> GetOrdersByUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            var userId = Guid.Parse(userIdClaim.Value);
            var orders = await orderRepository.GetOrdersByUserIdAsync(userId);
            var result = mapper.Map<List<OrderDto>>(orders);

            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrdersByUserId(Guid userId)
        {
            var orders = await orderRepository.GetOrdersByUserIdAsync(userId);
            var result = mapper.Map<List<OrderDto>>(orders);
            return Ok(result);
        }

        [HttpPost("refund")]
        public async Task<IActionResult> RefundOrderItem([FromBody] RefundOrderItemDto dto)
        {
            var result = await orderRepository.RefundOrderItemAsync(dto);
            if (!result) return BadRequest("Refund failed.");

            return Ok("Refund Succeseded.");
        }
    }
}
