using System.ComponentModel.DataAnnotations;

namespace RecipeManager.Infrastructure.DTOs;

public class IngredientDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public int SortOrder { get; set; }
}

public class CreateIngredientDto
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

public class StepDto
{
    public Guid Id { get; set; }
    public string Instruction { get; set; } = string.Empty;
    public int StepNumber { get; set; }
}

public class CreateStepDto
{
    [Required]
    public string Instruction { get; set; } = string.Empty;
}
