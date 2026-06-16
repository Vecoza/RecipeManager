using Microsoft.AspNetCore.Identity;

namespace RecipeManager.Infrastructure.Entities;

public class AppUser : IdentityUser
{
    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    public ICollection<PantryItem> PantryItems { get; set; } = new List<PantryItem>();
}
