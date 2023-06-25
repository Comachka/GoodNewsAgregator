using myProject.Core.DTOs;

namespace myProject.Abstractions.Services
{
    public interface IArticleService
    {
        Task<List<ArticleDto>> GetArticlesByPageAsync(int page, int pageSize, double positivity);
        Task<List<ArticleDto>> GetFavArticleAsync();
        Task<int> GetIdOfArticleASync(ArticleDto article);
        Task<int> GetTotalArticlesCountAsync(double raiting);
        Task<ArticleDto?> GetArticleByIdWithSourceNameAsync(int id);
        Task<List<AutoCompleteDataDto>> GetArticlesNamesByPartNameAsync(string partName);
        Task AddAsync(ArticleDto dto);
        Task EditArticleAsync(ArticleDto article);
        Task DeleteArticleByIdAsync(int id);
        Task UpRaitingAsync(int id);
        //aggregator
        Task AggregateArticlesDataFromRssAsync(CancellationToken cancellationToken);
        Task AddFullContentForArticlesAsync(CancellationToken cancellationToken);
        Task AddRaitingForArticlesAsync(CancellationToken cancellationToken);
    }
}