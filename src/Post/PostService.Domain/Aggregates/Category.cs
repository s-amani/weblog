using PostService.Domain.Aggregates;
using Weblog.Shared.Entities;

namespace PostService.Domain.Aggreagates;

public class Category : BaseEntity<Guid>
{
    public string Title { get; private set; }

    public IList<Post> Posts { get; private set; }

    public Category(string title)
    {
        Posts = new List<Post>();

        SetTitle(title);
    }

    public void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Category name cannot be empty", nameof(title));

        Title = title;
    }

    public void RemoveAssociatedPosts()
    {
        Posts.Clear();
    }

    public bool IsEqual(Category other)
    {
        if (other == null) return false;
        return Title.Equals(other.Title, StringComparison.OrdinalIgnoreCase);
    }
}