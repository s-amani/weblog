using System;
using System.Threading;
using System.Threading.Tasks;
using Weblog.Shared.Entities;
using Weblog.Shared.Interfaces;

namespace PostService.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    public IRepositoryBase<TKey, TEntity> Repository<TKey, TEntity>() where TEntity: BaseEntity<TKey>;

    Task<int> CommitAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
