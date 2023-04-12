using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using myProject.Core.DTOs;
using myProject.Data.Entities;

namespace myProject.Abstractions.Data.Repositories
{
    public interface ICommentRepository : IRepository<Comment>
    {
        public Task<List<Comment>> GetCommentsForPageAsync(int page, int pageSize);
    }
}
