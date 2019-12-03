using GrowRoomEnvironment.Contracts.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GrowRoomEnvironment.DataAccess.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _entities;

        public Repository(DbContext context)
        {
            _context = context;
            _entities = context.Set<TEntity>();
        }

        public virtual EntityEntry<TEntity> Add(TEntity entity)
        {
            return _entities.Add(entity);
        }

        public async virtual Task<EntityEntry<TEntity>> AddAsync(TEntity entity)
        {
           return  await _entities.AddAsync(entity);
        }


        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            _entities.AddRange(entities);
        }

        public async virtual Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
           await _entities.AddRangeAsync(entities);
        }

        public virtual void Update(TEntity entity)
        {
            _entities.Update(entity);
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities)
        {
            _entities.UpdateRange(entities);
        }

        public virtual void Remove(TEntity entity)
        {
            _entities.Remove(entity);
        }

        public virtual void RemoveRange(IEnumerable<TEntity> entities)
        {
            _entities.RemoveRange(entities);
        }


        public virtual int Count()
        {
            return _entities.Count();
        }

        public async virtual Task<int> CountAsync()
        {
            return await _entities.CountAsync();
        }

        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, int pageNumber = -1, int pageSize = -1)
        {
            IQueryable<TEntity> query = _entities.Where(predicate);

            if (pageNumber != -1 && pageSize != -1)
                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return query.ToList();
        }

        public async virtual Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, int pageNumber = -1, int pageSize = -1)
        {
            IQueryable<TEntity> query = _entities.Where(predicate);
            
            if (pageNumber != -1 && pageSize != -1)
                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return await query.ToListAsync();
        }

        public virtual TEntity GetSingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return _entities.SingleOrDefault(predicate);
        }

        public async virtual Task<TEntity> GetSingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _entities.SingleOrDefaultAsync(predicate);
        }

        public virtual TEntity Get(int id)
        {
            return _entities.Find(id);
        }

        public async virtual Task<TEntity> GetAsync(int id)
        {
            return await _entities.FindAsync(id);
        }

        public virtual IEnumerable<TEntity> GetAll(int pageNumber = -1, int pageSize = -1)
        {
            IQueryable<TEntity> query = _entities;

            if (pageNumber != -1 && pageSize != -1)
                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return query.ToList();
        }

        public async virtual Task<IEnumerable<TEntity>> GetAllAsync(int pageNumber = -1, int pageSize = -1)
        {
            IQueryable<TEntity> query = _entities;

            if (pageNumber != -1 && pageSize != -1)
                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return await query.ToListAsync();
        }
    }
}
