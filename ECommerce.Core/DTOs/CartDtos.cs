namespace ECommerce.Core.DTOs;

public record AddToCartDto(int ProductId, int Quantity);
public record CartItemDto(int ProductId, string ProductName, decimal UnitPrice, int Quantity, decimal Subtotal);
public record CartDto(List<CartItemDto> Items, decimal Total);