namespace RecipeManager.Infrastructure.Entities;

/// <summary>
/// Ingredient — child of Recipe (one-to-many).
/// SortOrder preserves the user-defined ingredient sequence.
/// Cascade delete is configured in AppDbContext.
/// </summary>
public class Ingredient : BaseEntity
{
    public Guid RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;  // g, ml, cup, tbsp, tsp, pcs …
    public int SortOrder { get; set; }
}
