using System;
using System.ComponentModel.DataAnnotations;

namespace PostService.Application.DTOs.Post;

public class PostUpdateDTO
{

    [Required]
    [MaxLength(256)]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }
}
