using ECommerce.Api.Extensions;
using ECommerce.Core.DTOs;
using ECommerce.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _service;

    public OrdersController(IOrderService service) => _service = service;

    [HttpPost("checkout")]
    public async Task<ActionResult<OrderDto>> Checkout()
    {
        var (order, error) = await _service.CheckoutAsync(User.GetUserId());
        return error is null ? Created($"/api/orders/{order!.Id}", order) : BadRequest(error);
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetMine() =>
        Ok(await _service.GetMyOrdersAsync(User.GetUserId()));

    [HttpPost("{id}/pay")]
    public async Task<ActionResult<OrderDto>> Pay(int id)
    {
        var (order, error) = await _service.PayAsync(User.GetUserId(), id);
        return error is null ? Ok(order) : BadRequest(error);
    }
}