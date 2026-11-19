using ECommerce.Core.DTOs;
using ECommerce.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/orders")]
public class AdminOrdersController : ControllerBase
{
    private readonly IOrderService _service;

    public AdminOrdersController(IOrderService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<AdminOrderDto>>> GetAll() =>
        Ok(await _service.GetAllAsync());

    [HttpPut("{id}/status")]
    public async Task<ActionResult<AdminOrderDto>> UpdateStatus(int id, UpdateOrderStatusDto dto)
    {
        var (order, error) = await _service.UpdateStatusAsync(id, dto.Status);
        return error is null ? Ok(order) : BadRequest(error);
    }
}