namespace RecipeManager.Infrastructure.Entities;

public class Step : BaseEntity
{
    public Guid RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    public string Instruction { get; set; } = string.Empty;
    public int StepNumber { get; set; }
}
