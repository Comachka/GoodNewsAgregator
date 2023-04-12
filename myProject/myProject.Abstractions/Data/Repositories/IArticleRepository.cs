using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using myProject.Data.Entities;

namespace myProject.Abstractions.Data.Repositories
{
    public interface IArticleRepository : IRepository<Article>
    {
        public Task<List<Article>> GetArticlesForPageAsync(int page, int pageSize);
    }
}
