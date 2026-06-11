using RecipeManager.Infrastructure.Entities;

namespace RecipeManager.Infrastructure.Interfaces;

public interface ITokenService
{
    string CreateToken(AppUser user);
}
