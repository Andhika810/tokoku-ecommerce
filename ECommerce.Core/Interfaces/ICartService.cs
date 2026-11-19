using ECommerce.Core.DTOs;

namespace ECommerce.Core.Interfaces;

public interface ICartService
{
    Task<CartDto> GetAsync(int userId);
    Task<string?> AddAsync(int userId, AddToCartDto dto);
    Task<bool> RemoveAsync(int userId, int productId);
}