using System.Security.Cryptography.X509Certificates;
using PostService.Domain.Aggregates;
using PostService.Domain.Repositories;
using PostService.Infrastructure.Persistence;

namespace PostService.Infrastructure.Repositories;

public class PostRepository : GenericRepository<Guid, Post>, IPostRepository
{
    public PostRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public IEnumerable<Post> GetPostsByCategoryId(Guid categoryId)
    {
        return _dbContext.Posts.Where(x => x.CategoryId == categoryId);
    }
}
