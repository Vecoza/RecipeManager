using RecipeManager.Infrastructure.DTOs;

namespace RecipeManager.Infrastructure.Interfaces;

public interface ITagService
{
    Task<IEnumerable<TagDto>> GetAllAsync();
    Task<TagDto> CreateAsync(CreateTagDto dto);
}
