namespace RecipeManager.Infrastructure.Entities;

/// <summary>
/// Recipe — the core aggregate of the application.
/// Owned by one AppUser. Has one-to-many Ingredients and Steps,
/// and many-to-many Tags (via RecipeTags join table).
/// </summary>
public class Recipe : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public AppUser User { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    public int PrepTimeMinutes { get; set; }
    public int CookTimeMinutes { get; set; }
    public int Servings { get; set; } = 4;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
    public ICollection<Step> Steps { get; set; } = new List<Step>();
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
