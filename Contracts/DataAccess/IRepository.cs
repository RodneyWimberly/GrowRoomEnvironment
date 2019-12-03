using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
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

        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);

        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);

        int Count();
        Task<int> CountAsync();

        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, int pageNumber = -1, int pageSize = -1);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, int pageNumber = -1, int pageSize = -1);
        TEntity GetSingleOrDefault(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> GetSingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        TEntity Get(int id);
        Task<TEntity> GetAsync(int id);
        IEnumerable<TEntity> GetAll(int pageNumber = -1, int pageSize = -1);
        Task<IEnumerable<TEntity>> GetAllAsync(int pageNumber = -1, int pageSize = -1);
    }
}
