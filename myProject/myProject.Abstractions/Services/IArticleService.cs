using myProject.Core.DTOs;

namespace myProject.Abstractions.Services
{
    public interface IArticleService
    {
        public Task<IEnumerable<CategoryDto>> GetListCategoriesAsync();
        public Task<List<ArticleDto>> GetArticlesByPageAsync(int page, int pageSize, double positivity);
        public Task<List<ArticleDto>> GetFavArticleAsync();
        public Task<int> GetTotalArticlesCountAsync();
        public Task<ArticleDto?> GetArticleByIdWithSourceNameAsync(int id);
        public Task<List<AutoCompleteDataDto>> GetArticlesNamesByPartNameAsync(string partName);
        public Task AddAsync(ArticleDto dto);
        public Task AddArticlesAsync(IEnumerable<ArticleDto> articles);
        //aggregator
        Task<List<ArticleDto>> AggregateArticlesDataFromRssSourceAsync(NewsResourceDto source,
           CancellationToken cancellationToken);
        Task<List<ArticleDto>> GetFullContentArticlesAsync(List<ArticleDto> articlesDataFromRss);
        Task<double?> GetArticleRateAsync(int articleId);
        Task<List<ArticleDto>> GetUnratedArticlesAsync();
        Task RateArticleAsync(int id, double? rate);
        Task DeleteArticleByIdAsync(int id);
        Task UpRaitingAsync(int id);
    }
}