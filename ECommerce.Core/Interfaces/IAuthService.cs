using ECommerce.Core.DTOs;

namespace ECommerce.Core.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);   // null = email sudah dipakai
    Task<AuthResponseDto?> LoginAsync(LoginDto dto);         // null = email/password salah
}