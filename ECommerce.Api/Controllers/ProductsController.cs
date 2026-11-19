using ECommerce.Core.DTOs;
using ECommerce.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetAll() =>
        Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await _service.GetByIdAsync(id);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Nama produk tidak boleh kosong.");
        if (dto.Price <= 0)
            return BadRequest("Harga harus lebih dari 0.");
        if (dto.Stock < 0)
            return BadRequest("Stok tidak boleh negatif.");

        var created = await _service.CreateAsync(dto);
        return created is null
            ? BadRequest("Kategori tidak ditemukan.")
            : Created($"/api/products/{created.Id}", created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> Update(int id, UpdateProductDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Nama produk tidak boleh kosong.");
        if (dto.Price <= 0)
            return BadRequest("Harga harus lebih dari 0.");
        if (dto.Stock < 0)
            return BadRequest("Stok tidak boleh negatif.");

        var updated = await _service.UpdateAsync(id, dto);
        return updated is null ? NotFound("Produk atau kategori tidak ditemukan.") : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id) =>
        await _service.DeactivateAsync(id) ? NoContent() : NotFound();
}