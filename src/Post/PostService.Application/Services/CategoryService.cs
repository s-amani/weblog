using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Weblog.Shared.Interfaces;
using PostService.Application.Interfaces;
using PostService.Application.DTOs.Category;
using PostService.Domain.Aggreagates;

namespace CategoryService.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IRepositoryBase<Guid, Category> _repository;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _mapper = mapper;
        _uow = unitOfWork;
        _repository = unitOfWork.Repository<Guid, Category>();
    }

    public async Task<Category> Create(CategoryCreateDTO dto)
    {
        var category = new Category(title: dto.Title);

        _repository.Add(category);

        await _uow.CommitAsync();

        return category;
    }

    public async Task<Category> Update(Guid id, CategoryUpdateDTO categoryDTO)
    {
        var category = await _repository.Get(id);

        category.SetTitle(categoryDTO.Title);

        _repository.Update(category);
    
        await _uow.CommitAsync();

        return category;
    }

    public async Task Delete(Guid id)
    {
        var category = await _repository.Get(id);

        category.RemoveAssociatedPosts();

        _repository.Remove(category);

        await _uow.CommitAsync();
    }

    public async Task<CategoryReadDTO> Get(Guid id) => _mapper.Map<CategoryReadDTO>(await _repository.Get(id));

    public async Task<IEnumerable<CategoryReadDTO>> Get() => _mapper.Map<IEnumerable<CategoryReadDTO>>(await _repository.Get().ToListAsync());

}
