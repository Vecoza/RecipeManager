using RecipeManager.Infrastructure.DTOs;

namespace RecipeManager.Infrastructure.Interfaces;

public interface IRecipeService
{
    Task<IEnumerable<RecipeListItemDto>> GetAllAsync(string userId, string? search, string? tags);
    Task<RecipeDetailDto?> GetByIdAsync(Guid id, string userId);
    Task<RecipeDetailDto> CreateAsync(string userId, CreateRecipeDto dto);
    Task<RecipeDetailDto?> UpdateAsync(Guid id, string userId, CreateRecipeDto dto);
    Task<bool> DeleteAsync(Guid id, string userId);
}
