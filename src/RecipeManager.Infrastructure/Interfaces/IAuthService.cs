using RecipeManager.Infrastructure.DTOs;

namespace RecipeManager.Infrastructure.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}
