using RecipeManager.Infrastructure.DTOs;

namespace RecipeManager.Infrastructure.Interfaces;

public interface IAiRecipeService
{
    Task<GeneratedRecipeDto> GenerateRecipeAsync(IEnumerable<PantryItemDto> pantryItems, GenerateRecipeRequestDto preferences);
}
