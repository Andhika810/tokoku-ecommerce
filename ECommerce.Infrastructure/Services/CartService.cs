using ECommerce.Core.DTOs;
using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Services;

public class CartService : ICartService
{
    private readonly AppDbContext _db;

    public CartService(AppDbContext db) => _db = db;

    public async Task<CartDto> GetAsync(int userId)
    {
        var items = await _db.CartItems
            .Where(c => c.UserId == userId)
            .Select(c => new CartItemDto(
                c.ProductId, c.Product!.Name, c.Product.Price,
                c.Quantity, c.Product.Price * c.Quantity))
            .ToListAsync();

        return new CartDto(items, items.Sum(i => i.Subtotal));
    }

    public async Task<string?> AddAsync(int userId, AddToCartDto dto)
    {
        if (dto.Quantity <= 0) return "Jumlah harus lebih dari 0.";

        var product = await _db.Products.FindAsync(dto.ProductId);
        if (product is null || !product.IsActive) return "Produk tidak ditemukan.";

        var existing = await _db.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == dto.ProductId);

        var newQty = (existing?.Quantity ?? 0) + dto.Quantity;
        if (newQty > product.Stock) return $"Stok tidak cukup (tersisa {product.Stock}).";

        if (existing is null)
            _db.CartItems.Add(new CartItem { UserId = userId, ProductId = dto.ProductId, Quantity = dto.Quantity });
        else
            existing.Quantity = newQty;

        await _db.SaveChangesAsync();
        return null;
    }

    public async Task<bool> RemoveAsync(int userId, int productId)
    {
        var item = await _db.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
        if (item is null) return false;

        _db.CartItems.Remove(item);
        await _db.SaveChangesAsync();
        return true;
    }
}