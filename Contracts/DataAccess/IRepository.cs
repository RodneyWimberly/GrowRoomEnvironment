using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GrowRoomEnvironment.Contracts.DataAccess
{
    public interface IRepository<TEntity> where TEntity : class
    {
        EntityEntry<TEntity> Add(TEntity entity);
        Task<EntityEntry<TEntity>> AddAsync(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        Task AddRangeAsync(IEnumerable<TEntity> entities);

        EntityEntry<TEntity> Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);

        EntityEntry<TEntity> Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);

        int Count();
        Task<int> CountAsync();

        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, int pageNumber = -1, int pageSize = -1);
        Task<IList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, int pageNumber = -1, int pageSize = -1);
        TEntity GetSingleOrDefault(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> GetSingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        TEntity Get(int id);
        Task<TEntity> GetAsync(int id);
        IQueryable<TEntity> GetAll(int pageNumber = -1, int pageSize = -1);
        Task<IList<TEntity>> GetAllAsync(int pageNumber = -1, int pageSize = -1);
    }
}
