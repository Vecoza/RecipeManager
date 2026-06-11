using System.ComponentModel.DataAnnotations;

namespace RecipeManager.Infrastructure.DTOs;

public class TagDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CreateTagDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}
