using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using PostService.Application.DTOs.Post;
using PostService.Domain.Aggregates;
using PostService.Domain.ValueObjects;

namespace PostService.Application.Mappers;

public class PostProfile : Profile
{
    public PostProfile()
    {
        CreateMap<PostReadDTO, Post>();
        
        CreateMap<Post, PostReadDTO>()
            .ForMember(x => x.Tags, src => src.MapFrom(v => string.Join(",", v.Tags.Select(c => c.Name))));

        CreateMap<PostCreateDTO, Post>()
            .AfterMap((src, dest) => dest.Author = "Admin User") // temporary field, gonna be removed once we implemented user service
            .ForMember(x => x.Content, c => c.MapFrom(v => new PostContent { Text = v.Content }))
            .ForMember(x => x.Tags, c => c.MapFrom(v =>
                new List<Tag>(
                        v.Tags.Split(',', System.StringSplitOptions.RemoveEmptyEntries).Select(x => new Tag { Name = x })
                    )
                )
            );
    }
}
