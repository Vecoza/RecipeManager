using Microsoft.EntityFrameworkCore;
using RecipeManager.Infrastructure.DTOs;
using RecipeManager.Infrastructure.Data;
using RecipeManager.Infrastructure.Entities;
using RecipeManager.Infrastructure.Interfaces;

namespace RecipeManager.Infrastructure.Services;

public class TagService : ITagService
{
    private readonly AppDbContext _db;

    public TagService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<TagDto>> GetAllAsync()
    {
        return await _db.Tags
            .OrderBy(t => t.Name)
            .Select(t => new TagDto { Id = t.Id, Name = t.Name })
            .ToListAsync();
    }

    public async Task<TagDto> CreateAsync(CreateTagDto dto)
    {
        var normalised = dto.Name.Trim();

        var existing = await _db.Tags
            .FirstOrDefaultAsync(t => t.Name.ToLower() == normalised.ToLower());

        if (existing is not null)
            return new TagDto { Id = existing.Id, Name = existing.Name };

        var tag = new Tag { Name = normalised };
        _db.Tags.Add(tag);
        await _db.SaveChangesAsync();

        return new TagDto { Id = tag.Id, Name = tag.Name };
    }
}
