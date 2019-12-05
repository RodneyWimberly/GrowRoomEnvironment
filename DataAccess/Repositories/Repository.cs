using GrowRoomEnvironment.Contracts.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GrowRoomEnvironment.DataAccess.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext Context;
        protected readonly DbSet<TEntity> Entity;
        protected readonly DatabaseFacade Database;

       public Repository(DbContext context)
        {
            Context = context;
            Entity = context.Set<TEntity>();
            Database = Context.Database;
        }

        public virtual EntityEntry<TEntity> Add(TEntity entity)
        {
            return Entity.Add(entity);
        }

        public async virtual Task<EntityEntry<TEntity>> AddAsync(TEntity entity)
        {
           return  await Entity.AddAsync(entity);
        }


        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            Entity.AddRange(entities);
        }

        public async virtual Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
           await Entity.AddRangeAsync(entities);
        }

        public virtual void Update(TEntity entity)
        {
            Entity.Update(entity);
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities)
        {
            Entity.UpdateRange(entities);
        }

        public virtual void Remove(TEntity entity)
        {
            Entity.Remove(entity);
        }

        public virtual void RemoveRange(IEnumerable<TEntity> entities)
        {
            Entity.RemoveRange(entities);
        }


        public virtual int Count()
        {
            return Entity.Count();
        }

        public async virtual Task<int> CountAsync()
        {
            return await Entity.CountAsync();
        }

        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate, int pageNumber = -1, int pageSize = -1)
        {
            IQueryable<TEntity> query = Entity.Where(predicate);

            if (pageNumber != -1 && pageSize != -1)
                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return query.ToList();
        }

        public async virtual Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, int pageNumber = -1, int pageSize = -1)
        {
            IQueryable<TEntity> query = Entity.Where(predicate);
            
            if (pageNumber != -1 && pageSize != -1)
                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return await query.ToListAsync();
        }

        public virtual TEntity GetSingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Entity.SingleOrDefault(predicate);
        }

        public async virtual Task<TEntity> GetSingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await Entity.SingleOrDefaultAsync(predicate);
        }

        public virtual TEntity Get(int id)
        {
            return Entity.Find(id);
        }

        public async virtual Task<TEntity> GetAsync(int id)
        {
            return await Entity.FindAsync(id);
        }

        public virtual IEnumerable<TEntity> GetAll(int pageNumber = -1, int pageSize = -1)
        {
            IQueryable<TEntity> query = Entity;

            if (pageNumber != -1 && pageSize != -1)
                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return query.ToList();
        }

        public async virtual Task<IEnumerable<TEntity>> GetAllAsync(int pageNumber = -1, int pageSize = -1)
        {
            IQueryable<TEntity> query = Entity;

            if (pageNumber != -1 && pageSize != -1)
                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            return await query.ToListAsync();
        }
    }
}
