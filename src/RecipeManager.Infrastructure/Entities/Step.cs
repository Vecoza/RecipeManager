namespace RecipeManager.Infrastructure.Entities;

/// <summary>
/// Step — child of Recipe (one-to-many).
/// StepNumber preserves the user-defined instruction order.
/// Cascade delete is configured in AppDbContext.
/// </summary>
public class Step : BaseEntity
{
    public Guid RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    public string Instruction { get; set; } = string.Empty;
    public int StepNumber { get; set; }
}
