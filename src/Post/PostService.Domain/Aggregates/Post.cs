using PostService.Domain.Aggreagates;
using PostService.Domain.Entities;
using PostService.Domain.ValueObjects;
using Weblog.Shared.Entities;

namespace PostService.Domain.Aggregates;

public class Post : BaseEntity<Guid>
{
    public string Title { get; set; }
    public PostContent Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsPublished { get; set; } = false;
    public DateTime? PublishedAt { get; set; }
    public string Author { get; set; }

    public List<Tag> Tags { get; set; }
    public List<Comment> Comments { get; set; }

    public virtual Guid? CategoryId { get; set; }

    public virtual Guid? AuthorId { get; set; }

    public void EditTitle(string newTitle)
    {
        if (!string.IsNullOrWhiteSpace(newTitle))
        {
            Title = newTitle;
            UpdateTimestamp();
        }
    }

    public void EditContent(PostContent newContent)
    {
        Content = newContent ?? throw new ArgumentNullException(nameof(newContent));
        UpdateTimestamp();
    }

    public void Publish()
    {
        IsPublished = true;
        PublishedAt = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void Unpublish()
    {
        IsPublished = false;
        PublishedAt = null;
        UpdateTimestamp();
    }

    public void UpdateTags(List<Tag> newTags)
    {
        Tags = newTags ?? throw new ArgumentNullException(nameof(newTags));
        UpdateTimestamp();
    }

    private void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangePublishStatus(bool isPublished)
    {
        IsPublished = isPublished;
    }

    public void AddComment(string content)
    {
        if (Comments is null)
            Comments = new List<Comment>();

        var comment  = new Comment(content, Id);
    
        Comments.Add(comment);
    }

    public void EditComment(Guid commentId, string content)
    {
        var comment  = Comments.FirstOrDefault(x => x.Id == commentId);

        if (comment is null) return;

        comment.EditContent(content);
    }

    public void RemoveComment(Guid commentId)
    {
        var comment = Comments.FirstOrDefault(x => x.Id == commentId);

        if (comment is null) return;

        Comments.Remove(comment);
    }
}