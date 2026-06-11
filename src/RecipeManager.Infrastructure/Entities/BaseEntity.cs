namespace RecipeManager.Infrastructure.Entities;

/// <summary>
/// Base class for all entities. Provides a common UUID primary key
/// so every entity follows the same identity contract.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}
