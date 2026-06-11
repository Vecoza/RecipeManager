namespace RecipeManager.Infrastructure.Entities;

/// <summary>
/// Tag — shared across all users (e.g. Vegetarian, Quick, Dessert).
/// Many-to-many with Recipe via the RecipeTags join table.
/// </summary>
public class Tag : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}
