using ECommerce.Core.DTOs;
using ECommerce.Core.Entities;
using ECommerce.Core.Interfaces;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _db;

    public ProductService(AppDbContext db) => _db = db;

    public async Task<List<ProductDto>> GetAllAsync() =>
        await _db.Products
            .Where(p => p.IsActive)
            .Select(p => new ProductDto(p.Id, p.Name, p.Description, p.Price, p.Stock, p.CategoryId, p.Category!.Name, p.ImageUrl))
            .ToListAsync();

    public async Task<ProductDto?> GetByIdAsync(int id) =>
        await _db.Products
            .Where(p => p.Id == id && p.IsActive)
            .Select(p => new ProductDto(p.Id, p.Name, p.Description, p.Price, p.Stock, p.CategoryId, p.Category!.Name, p.ImageUrl))
            .FirstOrDefaultAsync();

    public async Task<ProductDto?> CreateAsync(CreateProductDto dto)
    {
        var category = await _db.Categories.FindAsync(dto.CategoryId);
        if (category is null) return null;

        var product = new Product
        {
            Name = dto.Name.Trim(),
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            CategoryId = dto.CategoryId,
            ImageUrl = dto.ImageUrl
        };
        _db.Products.Add(product);
        await _db.SaveChangesAsync();

        return new ProductDto(product.Id, product.Name, product.Description, product.Price, product.Stock, product.CategoryId, category.Name, product.ImageUrl);
    }

    public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return null;

        var category = await _db.Categories.FindAsync(dto.CategoryId);
        if (category is null) return null;

        product.Name = dto.Name.Trim();
        product.Description = dto.Description;
        product.Price = dto.Price;
        product.Stock = dto.Stock;
        product.CategoryId = dto.CategoryId;
        product.IsActive = dto.IsActive;
        product.ImageUrl = dto.ImageUrl;
        await _db.SaveChangesAsync();

        return new ProductDto(product.Id, product.Name, product.Description, product.Price, product.Stock, product.CategoryId, category.Name, product.ImageUrl);
    }

    public async Task<bool> DeactivateAsync(int id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return false;

        product.IsActive = false;   // soft delete
        await _db.SaveChangesAsync();
        return true;
    }
}