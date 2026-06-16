using System.ComponentModel.DataAnnotations;

namespace RecipeManager.Infrastructure.DTOs;

public class PantryItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}

public class CreatePantryItemDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Range(0, double.MaxValue)]
    public decimal Quantity { get; set; }

    [MaxLength(50)]
    public string Unit { get; set; } = string.Empty;
}

public class RecipeMatchDto
{
    public RecipeListItemDto Recipe { get; set; } = null!;
    public int MatchedCount { get; set; }
    public int TotalCount { get; set; }
    public int MatchPercent => TotalCount == 0 ? 0 : (int)Math.Round((double)MatchedCount / TotalCount * 100);
}

public class PantryMatchResultDto
{
    public List<RecipeMatchDto> ExactMatches { get; set; } = [];
    public List<RecipeMatchDto> PartialMatches { get; set; } = [];
}

public class GeneratedRecipeDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int PrepTimeMinutes { get; set; }
    public int CookTimeMinutes { get; set; }
    public int Servings { get; set; }
    public List<CreateIngredientDto> Ingredients { get; set; } = [];
    public List<CreateStepDto> Steps { get; set; } = [];
}

public class GenerateRecipeRequestDto
{
    public string RecipeType { get; set; } = "Either";
    public string MealType { get; set; } = "Dinner";
    public string CookingTime { get; set; } = "Any";
}
