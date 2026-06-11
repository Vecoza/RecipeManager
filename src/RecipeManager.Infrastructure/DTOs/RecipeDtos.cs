using System.ComponentModel.DataAnnotations;

namespace RecipeManager.Infrastructure.DTOs;

public class RecipeListItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int PrepTimeMinutes { get; set; }
    public int CookTimeMinutes { get; set; }
    public int Servings { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<TagDto> Tags { get; set; } = [];
}

public class RecipeDetailDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int PrepTimeMinutes { get; set; }
    public int CookTimeMinutes { get; set; }
    public int Servings { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<IngredientDto> Ingredients { get; set; } = [];
    public List<StepDto> Steps { get; set; } = [];
    public List<TagDto> Tags { get; set; } = [];
}

public class CreateRecipeDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    [Range(0, int.MaxValue)]
    public int PrepTimeMinutes { get; set; }

    [Range(0, int.MaxValue)]
    public int CookTimeMinutes { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Servings { get; set; } = 4;

    [MinLength(1, ErrorMessage = "A recipe must have at least one ingredient.")]
    public List<CreateIngredientDto> Ingredients { get; set; } = [];

    [MinLength(1, ErrorMessage = "A recipe must have at least one step.")]
    public List<CreateStepDto> Steps { get; set; } = [];

    public List<Guid> TagIds { get; set; } = [];
}
