using myProject.Core.DTOs;

namespace myProject.Abstractions.Services
{
    public interface IArticleService
    {
        public Task<IEnumerable<CategoryDto>> GetListCategoriesAsync();
    }
}