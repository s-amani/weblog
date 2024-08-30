using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PostService.Application.DTOs.Category;
using PostService.Domain.Aggreagates;

namespace PostService.Application.Interfaces;

public interface ICategoryService
{
    Task<Category> Create(CategoryCreateDTO dto);
    Task<CategoryReadDTO> Get(Guid id);
    Task<IEnumerable<CategoryReadDTO>> Get();
    Task<Category> Update(Guid id, CategoryUpdateDTO postDTO);
}
