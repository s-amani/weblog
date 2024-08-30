using PostService.Domain.Aggreagates;
using Weblog.Shared.Interfaces;

namespace PostService.Domain.Repositories;

public interface ICategoryRepository : IRepositoryBase<Guid, Category>
{
}
