using System;

namespace PostService.Application.DTOs;

public class PostReadDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsPublished { get; set; } = false;
    public DateTime? PublishedAt { get; set; }
    public string Tags { get; set; }
    public virtual Guid? CategoryId { get; set; }
}
