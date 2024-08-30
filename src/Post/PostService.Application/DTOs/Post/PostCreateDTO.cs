using System;
using System.ComponentModel.DataAnnotations;

namespace PostService.Application.DTOs.Post;

public class PostCreateDTO
{

    [Required]
    [MaxLength(256)]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    [Required]
    [MaxLength(128)]
    public string Tags { get; set; }

    public Guid? CategoryId { get; set; }
}
