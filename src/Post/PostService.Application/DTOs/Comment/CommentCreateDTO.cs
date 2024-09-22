using System;

namespace PostService.Application.DTOs.Comment;

public class CommentCreateDTO
{
    public string Content { get; private set; }
    public Guid PostId { get; private set; }
}
