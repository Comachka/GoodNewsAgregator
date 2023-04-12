using Microsoft.EntityFrameworkCore;
using myProject.Abstractions.Data.Repositories;
using myProject.Core.DTOs;
using myProject.Data;
using myProject.Data.Entities;

namespace myProject.Repositories.Implementations
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(MyProjectContext myProjectContext)
            : base(myProjectContext)
        {
        }

        public async Task<List<Comment>> GetCommentsForPageAsync(int page, int pageSize)
        {
            var comments = await _dbSet
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return comments;
        }
      
    }
}