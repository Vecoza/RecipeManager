using Microsoft.AspNetCore.Identity;

namespace RecipeManager.Infrastructure.Entities;

/// <summary>
/// Application user — extends ASP.NET Identity's IdentityUser.
/// Navigation property gives access to all recipes owned by this user.
/// </summary>
public class AppUser : IdentityUser
{
    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}
