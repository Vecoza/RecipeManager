using Microsoft.AspNetCore.Identity;
using RecipeManager.Infrastructure.DTOs;
using RecipeManager.Infrastructure.Entities;
using RecipeManager.Infrastructure.Interfaces;

namespace RecipeManager.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;

    public AuthService(UserManager<AppUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser is not null)
            throw new InvalidOperationException("Email is already in use.");

        var user = new AppUser { UserName = dto.Email, Email = dto.Email };
        var result = await _userManager.CreateAsync(user, dto.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException(errors);
        }

        return new AuthResponseDto
        {
            Email = user.Email!,
            Token = _tokenService.CreateToken(user)
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email)
            ?? throw new UnauthorizedAccessException("Invalid email or password.");

        var isValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!isValid)
            throw new UnauthorizedAccessException("Invalid email or password.");

        return new AuthResponseDto
        {
            Email = user.Email!,
            Token = _tokenService.CreateToken(user)
        };
    }
}
