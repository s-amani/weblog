using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using PostService.Application.DTOs.Post;
using PostService.Domain.Aggregates;

namespace PostService.Application.Interfaces;

public interface IPostService
{
    Task<Post> Create(PostCreateDTO dto);
    Task<PostReadDTO> Get(Guid id);
    Task<IEnumerable<PostReadDTO>> Get();
    Task<Post> Update(Guid id, PostUpdateDTO postDTO);
    Task ChangePublishStatus(Guid value);
}
