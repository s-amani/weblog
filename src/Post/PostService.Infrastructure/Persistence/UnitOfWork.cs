using Microsoft.EntityFrameworkCore.Storage;
using PostService.Application.Interfaces;
using PostService.Domain.Repositories;
using PostService.Infrastructure.Repositories;
using Weblog.Shared.Entities;
using Weblog.Shared.Interfaces;

namespace PostService.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _dbContext;
    private IDbContextTransaction _dbTransaction;

    public IRepositoryBase<TKey, TEntity> Repository<TKey, TEntity>() where TEntity : BaseEntity<TKey> =>
        new GenericRepository<TKey, TEntity>(_dbContext);

    public UnitOfWork(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task BeginTransactionAsync()
    {
        _dbTransaction = await _dbContext.Database.BeginTransactionAsync();
    }

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default) => await _dbContext.SaveChangesAsync();

    public async Task CommitTransactionAsync()
    {
        if (_dbTransaction is not null)
        {
            await _dbTransaction.CommitAsync();
            await _dbTransaction.DisposeAsync();

            _dbTransaction = null;
        }
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        if (_dbTransaction != null)
        {
            _dbTransaction.Dispose();
            _dbTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_dbTransaction is not null)
        {
            await _dbTransaction.RollbackAsync();
            await _dbTransaction.DisposeAsync();

            _dbTransaction = null;
        }
    }
}
