using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Infrastructure.DTOs;
using RecipeManager.Infrastructure.Interfaces;

namespace RecipeManager.API.Controllers;

[Authorize]
public class PantryController : BaseApiController
{
    private readonly IPantryService _pantryService;
    private readonly IAiRecipeService _aiRecipeService;

    public PantryController(IPantryService pantryService, IAiRecipeService aiRecipeService)
    {
        _pantryService = pantryService;
        _aiRecipeService = aiRecipeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PantryItemDto>>> GetAll()
    {
        var items = await _pantryService.GetAllAsync(GetUserId());
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<PantryItemDto>> Create(CreatePantryItemDto dto)
    {
        var item = await _pantryService.CreateAsync(GetUserId(), dto);
        return Ok(item);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PantryItemDto>> Update(Guid id, CreatePantryItemDto dto)
    {
        var item = await _pantryService.UpdateAsync(id, GetUserId(), dto);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _pantryService.DeleteAsync(id, GetUserId());
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("matches")]
    public async Task<ActionResult<PantryMatchResultDto>> GetMatches()
    {
        var result = await _pantryService.FindMatchingRecipesAsync(GetUserId());
        return Ok(result);
    }

    [HttpPost("generate")]
    public async Task<ActionResult<GeneratedRecipeDto>> Generate([FromBody] GenerateRecipeRequestDto dto)
    {
        var pantryItems = await _pantryService.GetAllAsync(GetUserId());

        if (!pantryItems.Any())
            return BadRequest(new { message = "Add some ingredients to your pantry first." });

        try
        {
            var recipe = await _aiRecipeService.GenerateRecipeAsync(pantryItems, dto);
            return Ok(recipe);
        }
        catch (Exception ex)
        {
            return StatusCode(503, new { message = "AI generation failed. Make sure Ollama is running.", detail = ex.Message });
        }
    }
}
