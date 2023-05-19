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

        //public async Task AddArticleAsync(ArticleDto dto)
        //{
        //    await _myProjectContext.Articles
        //         .AddAsync(Convert(dto));
        //}

        //public async Task AddArticlesAsync(IEnumerable<ArticleDto> dtos)
        //{
        //    var entities = dtos.Select(dto => Convert(dto)).ToList();
        //    await _myProjectContext.Articles
        //         .AddRangeAsync(entities);
        //}

        //public async Task<ArticleDto?> GetArticleByIdAsync(int id)
        //{
        //    var article = await _myProjectContext.Articles
        //        .AsNoTracking()
        //        .FirstOrDefaultAsync(article => article.Id == id);
        //    if (article == null)
        //    {
        //        return null;
        //    }
        //    return Convert(article);
        //}

        //public async Task<List<ArticleDto>?> GetArticlesAsync()
        //{
        //    var articles = await _myProjectContext.Articles
        //          .AsNoTracking()
        //          .Select(article => Convert(article))
        //          .ToListAsync();
        //    if (articles == null)
        //    {
        //        return null;
        //    }
        //    return articles;
        //}

        //public async Task RemoveArticle(ArticleDto dto)
        //{
        //    var ent = Convert(dto);
        //    _myProjectContext.Articles.Remove(ent);
        //}

        //public async Task RemoveArticles(IEnumerable<ArticleDto> dtos)
        //{
        //    var ents = dtos.Select(dto => Convert(dto)).ToList();
        //    _myProjectContext.Articles.RemoveRange(ents);
        //}
        
        public async Task<List<Article>> GetArticlesForPageAsync(int page, int pageSize)
        {
            var articles = await _dbSet
                .Include(article => article.NewsResource)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return articles;
        }
       
        //private ArticleDto Convert(Article article)
        //{
        //    return new ArticleDto
        //    {
        //        Id = article.Id,
        //        Title = article.Title,
        //        Content = article.Content,
        //        PositiveRaiting = article.PositiveRaiting,
        //        DatePosting = article.DatePosting,
        //        NewsResourceId = article.NewsResourceId,
        //        CategoryId = article.CategoryId
        //    };
        //}
        //private Article Convert(ArticleDto dto)
        //{
        //    return new Article
        //    {
        //        Id = dto.Id,
        //        Title = dto.Title,
        //        Content = dto.Content,
        //        PositiveRaiting = dto.PositiveRaiting,
        //        DatePosting = dto.DatePosting,
        //        NewsResourceId = dto.NewsResourceId,
        //        CategoryId = dto.CategoryId
        //    };
        //}
    }
}