using System;
using System.ComponentModel.DataAnnotations;

namespace PostService.Application.DTOs.Category;

public class CategoryUpdateDTO
{

    [Required]
    [MaxLength(256)]
    public string Title { get; set; }
}
