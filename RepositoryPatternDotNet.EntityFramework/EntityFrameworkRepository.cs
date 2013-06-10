using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace RepositoryPatternDotNet.EntityFramework
{
    public sealed class EntityFrameworkRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        private readonly DbContext _dbContext;

        private readonly DbSet<TEntity> _dbSet;

        public EntityFrameworkRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<TEntity>();
        }

        IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator()
        {
            return ((IEnumerable<TEntity>) _dbSet).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _dbSet).GetEnumerator();
        }

        Expression IQueryable.Expression
        {
            get { return ((IQueryable) _dbSet).Expression; }
        }

        Type IQueryable.ElementType
        {
            get { return ((IQueryable) _dbSet).ElementType; }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return ((IQueryable) _dbSet).Provider; }
        }

        public void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public TEntity Find(params object[] primaryKeyValues)
        {
            return _dbSet.Find(primaryKeyValues);
        }

        public void Insert(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public void Update(TEntity entity)
        {
            var entityEntry = _dbContext.Entry(entity);

            switch (entityEntry.State)
            {
                case EntityState.Added:
                case EntityState.Modified:
                    return;

                case EntityState.Deleted:
                    throw new InvalidOperationException("Attempt to update an entity that has been marked for deletion.");

                case EntityState.Detached:
                    throw new InvalidOperationException("entityEntry.State is detached, immediately after being attached.");

                case EntityState.Unchanged:
                    entityEntry.State = EntityState.Modified;
                    return;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}