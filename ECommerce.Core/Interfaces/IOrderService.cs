using ECommerce.Core.DTOs;

namespace ECommerce.Core.Interfaces;

public interface IOrderService
{
    Task<(OrderDto? Order, string? Error)> CheckoutAsync(int userId);
    Task<List<OrderDto>> GetMyOrdersAsync(int userId);
    Task<(OrderDto? Order, string? Error)> PayAsync(int userId, int orderId);
    Task<List<AdminOrderDto>> GetAllAsync();
    Task<(AdminOrderDto? Order, string? Error)> UpdateStatusAsync(int orderId, string newStatus);
}