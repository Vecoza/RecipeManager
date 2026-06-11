using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Infrastructure.DTOs;
using RecipeManager.Infrastructure.Interfaces;

namespace RecipeManager.API.Controllers;

[Authorize]
public class RecipesController : BaseApiController
{
    private readonly IRecipeService _recipeService;

    public RecipesController(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RecipeListItemDto>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? tags)
    {
        var recipes = await _recipeService.GetAllAsync(GetUserId(), search, tags);
        return Ok(recipes);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RecipeDetailDto>> GetById(Guid id)
    {
        var recipe = await _recipeService.GetByIdAsync(id, GetUserId());
        return recipe is null ? NotFound() : Ok(recipe);
    }

    [HttpPost]
    public async Task<ActionResult<RecipeDetailDto>> Create(CreateRecipeDto dto)
    {
        var recipe = await _recipeService.CreateAsync(GetUserId(), dto);
        return CreatedAtAction(nameof(GetById), new { id = recipe.Id }, recipe);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<RecipeDetailDto>> Update(Guid id, CreateRecipeDto dto)
    {
        var recipe = await _recipeService.UpdateAsync(id, GetUserId(), dto);
        return recipe is null ? NotFound() : Ok(recipe);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _recipeService.DeleteAsync(id, GetUserId());
        return deleted ? NoContent() : NotFound();
    }
}
