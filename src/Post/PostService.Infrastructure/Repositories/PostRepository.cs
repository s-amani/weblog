using PostService.Domain.Aggregates;
using PostService.Domain.Repositories;
using PostService.Infrastructure.Persistence;

namespace PostService.Infrastructure.Repositories;

public class PostRepository : GenericRepository<Guid, Post>, IPostRepository
{
    public PostRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
