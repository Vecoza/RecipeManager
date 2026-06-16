using RecipeManager.Infrastructure.DTOs;

namespace RecipeManager.Infrastructure.Interfaces;

public interface IPantryService
{
    Task<IEnumerable<PantryItemDto>> GetAllAsync(string userId);
    Task<PantryItemDto> CreateAsync(string userId, CreatePantryItemDto dto);
    Task<PantryItemDto?> UpdateAsync(Guid id, string userId, CreatePantryItemDto dto);
    Task<bool> DeleteAsync(Guid id, string userId);
    Task<PantryMatchResultDto> FindMatchingRecipesAsync(string userId);
}
