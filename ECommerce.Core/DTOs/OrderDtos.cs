namespace ECommerce.Core.DTOs;

public record OrderItemDto(int ProductId, string ProductName, int Quantity, decimal UnitPrice, decimal Subtotal);
public record OrderDto(int Id, string Status, decimal TotalAmount, DateTime CreatedAt, List<OrderItemDto> Items);
public record AdminOrderDto(int Id, string CustomerName, string CustomerEmail, string Status, decimal TotalAmount, DateTime CreatedAt, List<OrderItemDto> Items);
public record UpdateOrderStatusDto(string Status);