using myProject.Core.DTOs;

namespace myProject.Abstractions.Services
{
    public interface IArticleService
    {
        public Task<IEnumerable<CategoryDto>> GetListCategoriesAsync();
        public Task<List<ArticleDto>> GetArticlesByPageAsync(int page, int pageSize);
        public Task<int> GetTotalArticlesCountAsync();
        public Task<ArticleDto?> GetArticleByIdWithSourceNameAsync(int id);
        Task<List<AutoCompleteDataDto>> GetArticlesNamesByPartNameAsync(string partName);
    }
}