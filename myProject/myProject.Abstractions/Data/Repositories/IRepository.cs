using System.Linq.Expressions;
using myProject.Core.DTOs;
using myProject.Core;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace myProject.Abstractions.Data.Repositories
{
    public interface IRepository <T> : IDisposable
        where T : class, IBaseEntity 
    {
        //read operations
        public Task<T> GetByIdAsync(int id);
        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
        public IQueryable<T> GetAsQueryable();
        //create operations
        Task<EntityEntry<T>> AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        Task PatchAsync(int id, List<PatchDto> patchDtos);
        Task Update(T entity);

        Task Remove(int id);
        Task RemoveRange(IEnumerable<T> entities);
        Task<int> CountAsync();
    }
}
