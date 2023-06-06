using AutoMapper;
using Microsoft.EntityFrameworkCore;
using myProject.Abstractions;
using myProject.Abstractions.Services;
using myProject.Core.DTOs;
using myProject.Data.Entities;


namespace myProject.Business
{
    public class SourceService : ISourceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SourceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<NewsResourceDto>> GetSourcesAsync()
        {
            return await _unitOfWork.NewsResources
                .GetAsQueryable()
                .Select(source => _mapper.Map<NewsResourceDto>(source))
                .ToListAsync();
        }

        public async Task<NewsResourceDto?> GetSourceIdsAsync(int id)
        {
            return _mapper.Map<NewsResourceDto>(await _unitOfWork.NewsResources.GetByIdAsync(id));
        }

        public async Task<int> AddSourceAsync(NewsResourceDto dto)
        {
            await _unitOfWork.NewsResources.AddAsync(_mapper.Map<NewsResource>(dto));
            return await _unitOfWork.SaveChangesAsync();

        }

    }
}