using ECommerce.Core.DTOs;
using ECommerce.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service) => _service = service;

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest("Nama dan email wajib diisi.");
        if (dto.Password is null || dto.Password.Length < 6)
            return BadRequest("Password minimal 6 karakter.");

        var result = await _service.RegisterAsync(dto);
        return result is null ? Conflict("Email sudah terdaftar.") : Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var result = await _service.LoginAsync(dto);
        return result is null ? Unauthorized("Email atau password salah.") : Ok(result);
    }
}