using Microsoft.EntityFrameworkCore;
using RecipeManager.Infrastructure.DTOs;
using RecipeManager.Infrastructure.Data;
using RecipeManager.Infrastructure.Entities;
using RecipeManager.Infrastructure.Interfaces;

namespace RecipeManager.Infrastructure.Services;

public class RecipeService : IRecipeService
{
    private readonly AppDbContext _db;

    public RecipeService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<RecipeListItemDto>> GetAllAsync(string userId, string? search, string? tags)
    {
        var query = _db.Recipes
            .Include(r => r.Tags)
            .Where(r => r.UserId == userId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(r => r.Title.ToLower().Contains(search.ToLower()));

        if (!string.IsNullOrWhiteSpace(tags))
        {
            var tagList = tags.Split(',', StringSplitOptions.RemoveEmptyEntries);
            query = query.Where(r => r.Tags.Any(t => tagList.Contains(t.Name)));
        }

        return await query
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new RecipeListItemDto
            {
                Id = r.Id,
                Title = r.Title,
                Description = r.Description,
                ImageUrl = r.ImageUrl,
                PrepTimeMinutes = r.PrepTimeMinutes,
                CookTimeMinutes = r.CookTimeMinutes,
                Servings = r.Servings,
                CreatedAt = r.CreatedAt,
                Tags = r.Tags.Select(t => new TagDto { Id = t.Id, Name = t.Name }).ToList()
            })
            .ToListAsync();
    }

    public async Task<RecipeDetailDto?> GetByIdAsync(Guid id, string userId)
    {
        var recipe = await _db.Recipes
            .Include(r => r.Ingredients.OrderBy(i => i.SortOrder))
            .Include(r => r.Steps.OrderBy(s => s.StepNumber))
            .Include(r => r.Tags)
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

        return recipe is null ? null : MapToDetailDto(recipe);
    }

    public async Task<RecipeDetailDto> CreateAsync(string userId, CreateRecipeDto dto)
    {
        var tags = await _db.Tags
            .Where(t => dto.TagIds.Contains(t.Id))
            .ToListAsync();

        var recipe = new Recipe
        {
            UserId = userId,
            Title = dto.Title,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            PrepTimeMinutes = dto.PrepTimeMinutes,
            CookTimeMinutes = dto.CookTimeMinutes,
            Servings = dto.Servings,
            Ingredients = MapIngredients(dto.Ingredients),
            Steps = MapSteps(dto.Steps),
            Tags = tags
        };

        _db.Recipes.Add(recipe);
        await _db.SaveChangesAsync();

        return MapToDetailDto(recipe);
    }

    public async Task<RecipeDetailDto?> UpdateAsync(Guid id, string userId, CreateRecipeDto dto)
    {
        var recipe = await _db.Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.Steps)
            .Include(r => r.Tags)
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

        if (recipe is null) return null;

        recipe.Title = dto.Title;
        recipe.Description = dto.Description;
        recipe.ImageUrl = dto.ImageUrl;
        recipe.PrepTimeMinutes = dto.PrepTimeMinutes;
        recipe.CookTimeMinutes = dto.CookTimeMinutes;
        recipe.Servings = dto.Servings;
        recipe.UpdatedAt = DateTime.UtcNow;

        _db.Ingredients.RemoveRange(recipe.Ingredients);
        _db.Steps.RemoveRange(recipe.Steps);
        recipe.Ingredients.Clear();
        recipe.Steps.Clear();

        var newIngredients = MapIngredients(dto.Ingredients);
        var newSteps = MapSteps(dto.Steps);
        foreach (var i in newIngredients) i.RecipeId = recipe.Id;
        foreach (var s in newSteps) s.RecipeId = recipe.Id;
        _db.Ingredients.AddRange(newIngredients);
        _db.Steps.AddRange(newSteps);

        var tags = await _db.Tags
            .Where(t => dto.TagIds.Contains(t.Id))
            .ToListAsync();
        recipe.Tags = tags;

        await _db.SaveChangesAsync();

        return MapToDetailDto(recipe);
    }

    public async Task<bool> DeleteAsync(Guid id, string userId)
    {
        var recipe = await _db.Recipes
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

        if (recipe is null) return false;

        _db.Recipes.Remove(recipe);
        await _db.SaveChangesAsync();
        return true;
    }

    private static List<Ingredient> MapIngredients(List<CreateIngredientDto> dtos) =>
        dtos.Select((dto, index) => new Ingredient
        {
            Name = dto.Name,
            Quantity = dto.Quantity,
            Unit = dto.Unit,
            SortOrder = index
        }).ToList();

    private static List<Step> MapSteps(List<CreateStepDto> dtos) =>
        dtos.Select((dto, index) => new Step
        {
            Instruction = dto.Instruction,
            StepNumber = index + 1
        }).ToList();

    private static RecipeDetailDto MapToDetailDto(Recipe r) => new()
    {
        Id = r.Id,
        UserId = r.UserId,
        Title = r.Title,
        Description = r.Description,
        ImageUrl = r.ImageUrl,
        PrepTimeMinutes = r.PrepTimeMinutes,
        CookTimeMinutes = r.CookTimeMinutes,
        Servings = r.Servings,
        CreatedAt = r.CreatedAt,
        UpdatedAt = r.UpdatedAt,
        Ingredients = r.Ingredients.Select(i => new IngredientDto
        {
            Id = i.Id,
            Name = i.Name,
            Quantity = i.Quantity,
            Unit = i.Unit,
            SortOrder = i.SortOrder
        }).ToList(),
        Steps = r.Steps.Select(s => new StepDto
        {
            Id = s.Id,
            Instruction = s.Instruction,
            StepNumber = s.StepNumber
        }).ToList(),
        Tags = r.Tags.Select(t => new TagDto { Id = t.Id, Name = t.Name }).ToList()
    };
}
