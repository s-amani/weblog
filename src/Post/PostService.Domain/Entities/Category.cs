using Microsoft.EntityFrameworkCore;
using PostService.Domain.Aggregates;
using Weblog.Shared.Entities;

namespace PostService.Domain.Entities;

public class Category: BaseEntity<Guid>
{
    public string Name { get; private set; }

    public virtual DbSet<Post> Posts { get; set; }

    public Category(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty", nameof(name));

        Name = name;
    }

    public bool IsEqual(Category other)
    {
        if (other == null) return false;
        return Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase);
    }
}