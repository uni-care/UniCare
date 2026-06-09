using UniCare.Domain.Common;

namespace UniCare.Domain.Aggregates.ItemAggregates;

public class Category : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    public virtual ICollection<Item> Items { get; private set; } = new List<Item>();

    private Category() { }

    public static Category Create(string name, string? description = null)
    {
        return new Category
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}
