namespace ECommerce.Core.DTOs;

public record ProductDto(int Id, string Name, string Description, decimal Price, int Stock, int CategoryId, string CategoryName, string? ImageUrl);
public record CreateProductDto(string Name, string Description, decimal Price, int Stock, int CategoryId, string? ImageUrl = null);
public record UpdateProductDto(string Name, string Description, decimal Price, int Stock, int CategoryId, bool IsActive, string? ImageUrl = null);