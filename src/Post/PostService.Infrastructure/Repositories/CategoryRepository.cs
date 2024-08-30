using PostService.Domain.Aggreagates;
using PostService.Domain.Repositories;
using PostService.Infrastructure.Persistence;

namespace PostService.Infrastructure.Repositories;

public class CategoryRepository : GenericRepository<Guid, Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}
