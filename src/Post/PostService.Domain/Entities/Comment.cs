using Weblog.Shared.Entities;

namespace PostService.Domain.Entities;

public class Comment : BaseEntity<Guid>
{
    public string AuthorName { get; private set; }
    public string Content { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public Guid PostId { get; private set; }

    public Comment(string content, Guid postId)
    {
        PostId = postId;
        Content = content;
        CreatedAt = DateTime.Now;
    }

    internal void EditContent(string content)
    {
        Content = content;
        UpdatedAt = DateTime.UtcNow;
    }
}