namespace RecipeManager.Infrastructure.Entities;

public class Tag : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}
