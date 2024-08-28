using System.Linq.Expressions;

namespace Weblog.Shared.Interfaces;

public interface IRepositoryBase<TKey, TEntity>
{
    void Add(in TEntity entity);
    void Update(in TEntity entity);
    Task Remove(TKey id);

    Task<TEntity> Get(TKey id);
    IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate = null);
}
