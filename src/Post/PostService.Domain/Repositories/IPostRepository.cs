using PostService.Domain.Aggregates;
using Weblog.Shared.Interfaces;

namespace PostService.Domain.Repositories;

public interface IPostRepository : IRepositoryBase<Guid, Post>
{
}
