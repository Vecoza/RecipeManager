using Microsoft.EntityFrameworkCore;
using RecipeManager.Infrastructure.DTOs;
using RecipeManager.Infrastructure.Data;
using RecipeManager.Infrastructure.Entities;
using RecipeManager.Infrastructure.Interfaces;

namespace RecipeManager.Infrastructure.Services;

public class PantryService : IPantryService
{
    private readonly AppDbContext _db;

    public PantryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<PantryItemDto>> GetAllAsync(string userId)
    {
        return await _db.PantryItems
            .Where(p => p.UserId == userId)
            .OrderBy(p => p.Name)
            .Select(p => new PantryItemDto
            {
                Id = p.Id,
                Name = p.Name,
                Quantity = p.Quantity,
                Unit = p.Unit
            })
            .ToListAsync();
    }

    public async Task<PantryItemDto> CreateAsync(string userId, CreatePantryItemDto dto)
    {
        var item = new PantryItem
        {
            UserId = userId,
            Name = dto.Name.Trim(),
            Quantity = dto.Quantity,
            Unit = dto.Unit.Trim()
        };

        _db.PantryItems.Add(item);
        await _db.SaveChangesAsync();

        return new PantryItemDto { Id = item.Id, Name = item.Name, Quantity = item.Quantity, Unit = item.Unit };
    }

    public async Task<PantryItemDto?> UpdateAsync(Guid id, string userId, CreatePantryItemDto dto)
    {
        var item = await _db.PantryItems.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
        if (item is null) return null;

        item.Name = dto.Name.Trim();
        item.Quantity = dto.Quantity;
        item.Unit = dto.Unit.Trim();

        await _db.SaveChangesAsync();

        return new PantryItemDto { Id = item.Id, Name = item.Name, Quantity = item.Quantity, Unit = item.Unit };
    }

    public async Task<bool> DeleteAsync(Guid id, string userId)
    {
        var item = await _db.PantryItems.FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
        if (item is null) return false;

        _db.PantryItems.Remove(item);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<PantryMatchResultDto> FindMatchingRecipesAsync(string userId)
    {
        var pantryItems = await _db.PantryItems
            .Where(p => p.UserId == userId)
            .Select(p => p.Name.ToLower())
            .ToListAsync();

        if (pantryItems.Count == 0)
            return new PantryMatchResultDto();

        var recipes = await _db.Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.Tags)
            .Where(r => r.UserId == userId)
            .ToListAsync();

        var exact = new List<RecipeMatchDto>();
        var partial = new List<RecipeMatchDto>();

        foreach (var recipe in recipes)
        {
            var recipeIngredientNames = recipe.Ingredients
                .Select(i => i.Name.ToLower())
                .ToList();

            var matched = recipeIngredientNames
                .Count(name => pantryItems.Any(p => name.Contains(p) || p.Contains(name)));

            var total = recipeIngredientNames.Count;
            if (total == 0) continue;

            var matchDto = new RecipeMatchDto
            {
                Recipe = new RecipeListItemDto
                {
                    Id = recipe.Id,
                    Title = recipe.Title,
                    Description = recipe.Description,
                    PrepTimeMinutes = recipe.PrepTimeMinutes,
                    CookTimeMinutes = recipe.CookTimeMinutes,
                    Servings = recipe.Servings,
                    CreatedAt = recipe.CreatedAt,
                    Tags = recipe.Tags.Select(t => new TagDto { Id = t.Id, Name = t.Name }).ToList()
                },
                MatchedCount = matched,
                TotalCount = total
            };

            if (matched == total)
                exact.Add(matchDto);
            else if (matched > 0)
                partial.Add(matchDto);
        }

        return new PantryMatchResultDto
        {
            ExactMatches = exact.OrderByDescending(m => m.MatchPercent).ToList(),
            PartialMatches = partial.OrderByDescending(m => m.MatchPercent).ToList()
        };
    }
}
