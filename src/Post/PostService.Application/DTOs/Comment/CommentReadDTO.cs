using System;

namespace PostService.Application.DTOs.Comment;

public class CommentReadDTO
{
    public Guid Id { get; set; }
    public string AuthorName { get; private set; }
    public string Content { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public Guid PostId { get; private set; }
}
