using myProject.Abstractions.Data;
using myProject.Abstractions.Data.Repositories;
using myProject.Core.DTOs;
using myProject.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using myProject.Core;

namespace myProject.Repositories.Implementations
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class, IBaseEntity
    {
        protected readonly MyProjectContext Db;
        protected readonly DbSet<TEntity> _dbSet;

        public Repository(MyProjectContext myProjectContext)
        {
            Db = myProjectContext;
            _dbSet = Db.Set<TEntity>();
        }

        public virtual void Dispose()
        {
            Db.Dispose();
            GC.SuppressFinalize(this);
        }

        public virtual async Task<EntityEntry<TEntity>> AddAsync(TEntity entity)
        {
            return await _dbSet.AddAsync(entity);
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public virtual IQueryable<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
        {
            var result = _dbSet.Where(predicate);
            if (includes.Any())
            {
                result = includes
                    .Aggregate(result, (current, include)
                    => current.Include(include));
            }
            return result;
        }

        public virtual IQueryable<TEntity> GetAsQueryable()
        {
            return _dbSet;
        }

        public virtual async Task<TEntity?> GetByIdAsync(int id)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(entity => entity.Id == id);
        }

        public virtual async Task PatchAsync(int id, List<PatchDto> patchDtos)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(ent => ent.Id == id);
            var nameValuePairProperties = patchDtos.ToDictionary
                (k => k.PropertyName,
                v => v.PropertyValue);
            var dbEntityEntry = Db.Entry(entity);
            dbEntityEntry.CurrentValues.SetValues(nameValuePairProperties);
            dbEntityEntry.State = EntityState.Modified;
        }


        public virtual async Task Remove(int id)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(ent => ent.Id == id);
            _dbSet.Remove(entity);
        }

        public virtual async Task RemoveRange(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public virtual async Task Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public virtual async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }
    }
}
