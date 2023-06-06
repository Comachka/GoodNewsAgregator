using myProject.Core.DTOs;

namespace myProject.Abstractions.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetCategoriesAsync();
    }
}