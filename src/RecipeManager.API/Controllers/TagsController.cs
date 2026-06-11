using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Infrastructure.DTOs;
using RecipeManager.Infrastructure.Interfaces;

namespace RecipeManager.API.Controllers;

public class TagsController : BaseApiController
{
    private readonly ITagService _tagService;

    public TagsController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<TagDto>>> GetAll()
    {
        var tags = await _tagService.GetAllAsync();
        return Ok(tags);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<TagDto>> Create(CreateTagDto dto)
    {
        var tag = await _tagService.CreateAsync(dto);
        return Ok(tag);
    }
}
