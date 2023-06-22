using myProject.Core.DTOs;
using myProject.Data.Entities;

namespace myProject.Abstractions.Services
{
    public interface ISourceService
    {
        Task<List<NewsResourceDto>> GetSourcesAsync();
        Task<NewsResourceDto?> GetSourceIdsAsync(int id);
        Task InitDefaultSourceAsync();
        Task<int> GetSourceIdByNameAsync(string source);
    }
}