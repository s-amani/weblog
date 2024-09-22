using System;

namespace PostService.Application.DTOs.Comment;

public class CommentUpdateDTO
{
    public Guid Id { get; set; }
    public string Content { get; private set; }
}
