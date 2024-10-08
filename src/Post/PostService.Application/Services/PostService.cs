using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PostService.Application.DTOs.Comment;
using PostService.Application.DTOs.Post;
using PostService.Application.Interfaces;
using PostService.Domain.Aggregates;
using PostService.Domain.Repositories;
using Weblog.Shared.Interfaces;

namespace PostService.Application.Services;

public class PostService : IPostService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IRepositoryBase<Guid, Post> _repository;

    public PostService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _mapper = mapper;
        _uow = unitOfWork;
        _repository = unitOfWork.Repository<Guid, Post>();
    }

    public async Task<Post> Create(PostCreateDTO dto)
    {
        var model = _mapper.Map<Post>(dto);
        _repository.Add(model);
        await _uow.CommitAsync();

        return model;
    }

    public async Task<Post> Update(Guid id, PostUpdateDTO postDTO)
    {
        var model = await _repository.Get(id);

        model.EditTitle(postDTO.Title);

        model.EditContent(new Domain.ValueObjects.PostContent{ Text = postDTO.Content });

        _repository.Update(model);

        await _uow.CommitAsync();

        return model;
    }

    public async Task<PostReadDTO> Get(Guid id) => _mapper.Map<PostReadDTO>(await _repository.Get(id));

    public async Task<IEnumerable<PostReadDTO>> Get() => _mapper.Map<IEnumerable<PostReadDTO>>(await _repository.Get().ToListAsync());

    public async Task ChangePublishStatus(Guid id)
    {
        var model = await _repository.Get(id);

        if (model is null) 
            throw new ArgumentNullException(nameof(model));

        model.ChangePublishStatus(isPublished: !model.IsPublished);

        await _uow.CommitAsync();
    }

    public async Task AddCommentToPostAsync(Guid postId, CommentCreateDTO comment)
    {
        var model = await _repository.Get(postId);

        if (model is null)
            throw new ArgumentNullException(nameof(model));

        model.AddComment(comment.Content);

        await _uow.CommitAsync();
    }

    public async Task UpdateCommentAsync(Guid postId, Guid commentId, CommentUpdateDTO comment)
    {
        var model = await _repository.Get(postId);
        
         if (model is null)
            throw new ArgumentNullException(nameof(model));

        model.EditComment(commentId, comment.Content);

        await _uow.CommitAsync();
    }

    public async Task DeleteCommentAsync(Guid postId, Guid commentId)
    {
         var model = await _repository.Get(postId);
        
         if (model is null)
            throw new ArgumentNullException(nameof(model));

        model.RemoveComment(commentId);

        await _uow.CommitAsync();
    }
}
