using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecipeManager.Infrastructure.Entities;

namespace RecipeManager.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<Step> Steps => Set<Step>();
    public DbSet<Tag> Tags => Set<Tag>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Recipe>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.Property(r => r.Title)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(r => r.Servings)
                  .HasDefaultValue(4);

            entity.HasOne(r => r.User)
                  .WithMany(u => u.Recipes)
                  .HasForeignKey(r => r.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(r => r.Ingredients)
                  .WithOne(i => i.Recipe)
                  .HasForeignKey(i => i.RecipeId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(r => r.Steps)
                  .WithOne(s => s.Recipe)
                  .HasForeignKey(s => s.RecipeId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(r => r.Tags)
                  .WithMany(t => t.Recipes)
                  .UsingEntity(j => j.ToTable("RecipeTags"));
        });

        builder.Entity<Ingredient>(entity =>
        {
            entity.HasKey(i => i.Id);

            entity.Property(i => i.Name)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(i => i.Quantity)
                  .HasColumnType("decimal(10,2)");

            entity.Property(i => i.Unit)
                  .HasMaxLength(50);
        });

        builder.Entity<Step>(entity =>
        {
            entity.HasKey(s => s.Id);

            entity.Property(s => s.Instruction)
                  .IsRequired();
        });

        builder.Entity<Tag>(entity =>
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Name)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.HasIndex(t => t.Name)
                  .IsUnique();
        });

        SeedTags(builder);
    }

    private static void SeedTags(ModelBuilder builder)
    {
        var defaultTags = new[]
        {
            "Vegetarian", "Vegan", "Quick", "Breakfast",
            "Lunch", "Dinner", "Dessert", "Snack",
            "Gluten-Free", "Dairy-Free"
        };

        var tags = defaultTags.Select((name, index) => new Tag
        {
            Id = Guid.Parse($"00000000-0000-0000-0000-{(index + 1):D12}"),
            Name = name
        }).ToArray();

        builder.Entity<Tag>().HasData(tags);
    }
}
