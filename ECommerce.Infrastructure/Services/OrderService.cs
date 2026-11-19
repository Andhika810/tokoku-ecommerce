using ECommerce.Core.DTOs;
using ECommerce.Core.Entities;
using ECommerce.Core.Enums;
using ECommerce.Core.Interfaces;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _db;

    public OrderService(AppDbContext db) => _db = db;

    public async Task<(OrderDto? Order, string? Error)> CheckoutAsync(int userId)
    {
        await using var tx = await _db.Database.BeginTransactionAsync();

        var cartItems = await _db.CartItems
            .Include(c => c.Product)
            .Where(c => c.UserId == userId)
            .ToListAsync();

        if (cartItems.Count == 0) return (null, "Keranjang kosong.");

        var order = new Order { UserId = userId };

        foreach (var item in cartItems)
        {
            var product = item.Product!;
            if (!product.IsActive)
                return (null, $"Produk '{product.Name}' sudah tidak tersedia.");
            if (product.Stock < item.Quantity)
                return (null, $"Stok '{product.Name}' tidak cukup (tersisa {product.Stock}).");

            product.Stock -= item.Quantity;
            order.Items.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            });
        }

        order.TotalAmount = order.Items.Sum(i => i.Quantity * i.UnitPrice);

        _db.Orders.Add(order);
        _db.CartItems.RemoveRange(cartItems);
        await _db.SaveChangesAsync();
        await tx.CommitAsync();

        return (await LoadOrderDto(order.Id), null);
    }

    public async Task<List<OrderDto>> GetMyOrdersAsync(int userId)
    {
        var ids = await _db.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => o.Id)
            .ToListAsync();

        var result = new List<OrderDto>();
        foreach (var id in ids)
            result.Add((await LoadOrderDto(id))!);
        return result;
    }

    public async Task<(OrderDto? Order, string? Error)> PayAsync(int userId, int orderId)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
        if (order is null) return (null, "Pesanan tidak ditemukan.");
        if (order.Status != OrderStatus.Pending) return (null, "Pesanan ini tidak bisa dibayar.");

        order.Status = OrderStatus.Paid;
        await _db.SaveChangesAsync();

        return (await LoadOrderDto(order.Id), null);
    }

    private async Task<OrderDto?> LoadOrderDto(int orderId) =>
        await _db.Orders
            .Where(o => o.Id == orderId)
            .Select(o => new OrderDto(
                o.Id,
                o.Status.ToString(),
                o.TotalAmount,
                o.CreatedAt,
                o.Items.Select(i => new OrderItemDto(
                    i.ProductId, i.Product!.Name, i.Quantity, i.UnitPrice, i.UnitPrice * i.Quantity)).ToList()))
            .FirstOrDefaultAsync();

            public async Task<List<AdminOrderDto>> GetAllAsync() =>
    await _db.Orders
        .OrderByDescending(o => o.CreatedAt)
        .Select(o => new AdminOrderDto(
            o.Id, o.User!.Name, o.User.Email, o.Status.ToString(), o.TotalAmount, o.CreatedAt,
            o.Items.Select(i => new OrderItemDto(
                i.ProductId, i.Product!.Name, i.Quantity, i.UnitPrice, i.UnitPrice * i.Quantity)).ToList()))
        .ToListAsync();

public async Task<(AdminOrderDto? Order, string? Error)> UpdateStatusAsync(int orderId, string newStatus)
{
    if (!Enum.TryParse<OrderStatus>(newStatus, true, out var target))
        return (null, "Status tidak valid.");

    await using var tx = await _db.Database.BeginTransactionAsync();

    var order = await _db.Orders
        .Include(o => o.Items)
        .FirstOrDefaultAsync(o => o.Id == orderId);
    if (order is null) return (null, "Pesanan tidak ditemukan.");

    var allowed = (order.Status, target) switch
    {
        (OrderStatus.Pending, OrderStatus.Cancelled) => true,
        (OrderStatus.Paid, OrderStatus.Shipped) => true,
        (OrderStatus.Paid, OrderStatus.Cancelled) => true,
        _ => false
    };
    if (!allowed)
        return (null, $"Status tidak bisa diubah dari {order.Status} ke {target}.");

    if (target == OrderStatus.Cancelled)
    {
        var productIds = order.Items.Select(i => i.ProductId).ToList();
        var products = await _db.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();
        foreach (var item in order.Items)
            products.First(p => p.Id == item.ProductId).Stock += item.Quantity;
    }

    order.Status = target;
    await _db.SaveChangesAsync();
    await tx.CommitAsync();

    var dto = await _db.Orders
        .Where(o => o.Id == orderId)
        .Select(o => new AdminOrderDto(
            o.Id, o.User!.Name, o.User.Email, o.Status.ToString(), o.TotalAmount, o.CreatedAt,
            o.Items.Select(i => new OrderItemDto(
                i.ProductId, i.Product!.Name, i.Quantity, i.UnitPrice, i.UnitPrice * i.Quantity)).ToList()))
        .FirstAsync();
    return (dto, null);
}
}