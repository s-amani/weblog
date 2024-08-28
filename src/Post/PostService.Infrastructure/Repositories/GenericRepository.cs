using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using PostService.Infrastructure.Persistence;
using Weblog.Shared.Entities;
using Weblog.Shared.Interfaces;

namespace PostService.Infrastructure.Repositories;

public class GenericRepository<TKey, TEntity> : IRepositoryBase<TKey, TEntity> where TEntity : BaseEntity<TKey>
{
    private readonly AppDbContext _dbContext;

    public GenericRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(in TEntity entity) => _dbContext.Add(entity).State = Microsoft.EntityFrameworkCore.EntityState.Added;
    public void Update(in TEntity entity) => _dbContext.Update(entity).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
    public async Task Remove(TKey id) => _dbContext.Remove(await _dbContext.FindAsync<TEntity>(id));

    public async Task<TEntity> Get(TKey id) => await _dbContext.Set<TEntity>().FindAsync(id);
    public IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate = null) 
    {
        var query = _dbContext.Set<TEntity>().AsQueryable().AsNoTracking();

        if (predicate is not null)
            query = query.Where(predicate);

        return query;
    } 


}
