using Microsoft.EntityFrameworkCore;
using myProject.Abstractions.Data.Repositories;
using myProject.Core.DTOs;
using myProject.Data;
using myProject.Data.Entities;

namespace myProject.Repositories.Implementations
{
    public class ArticleRepository :Repository<Article>, IArticleRepository
    {
        public ArticleRepository(MyProjectContext myProjectContext)
            : base(myProjectContext)
        {
        }

        
        public async Task<List<Article>> GetArticlesForPageAsync(int page, int pageSize)
        {
            var articles = await _dbSet
                .Include(article => article.NewsResource)
                .OrderByDescending(article => article.DatePosting)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return articles;
        }
       
    }
}