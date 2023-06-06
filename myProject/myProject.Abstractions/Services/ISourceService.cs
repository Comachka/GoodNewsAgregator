using myProject.Core.DTOs;

namespace myProject.Abstractions.Services
{
    public interface ISourceService
    {
        Task<List<NewsResourceDto>> GetSourcesAsync();
        Task<NewsResourceDto?> GetSourceIdsAsync(int id);
        Task<int> AddSourceAsync(NewsResourceDto dto);
    }
}