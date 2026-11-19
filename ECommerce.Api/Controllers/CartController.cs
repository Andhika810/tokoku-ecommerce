using ECommerce.Api.Extensions;
using ECommerce.Core.DTOs;
using ECommerce.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _service;

    public CartController(ICartService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<CartDto>> Get() =>
        Ok(await _service.GetAsync(User.GetUserId()));

    [HttpPost]
    public async Task<IActionResult> Add(AddToCartDto dto)
    {
        var error = await _service.AddAsync(User.GetUserId(), dto);
        return error is null ? Ok(await _service.GetAsync(User.GetUserId())) : BadRequest(error);
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> Remove(int productId) =>
        await _service.RemoveAsync(User.GetUserId(), productId) ? NoContent() : NotFound();
}