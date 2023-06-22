using myProject.DataCQS.Queries;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using myProject.Abstractions;
using myProject.Abstractions.Services;
using myProject.Core.DTOs;
using myProject.Data.Entities;
using MediatR;


namespace myProject.Business
{
    public class SourceService : ISourceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;


        public SourceService(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mediator = mediator;
            _configuration = configuration;
        }
        public async Task<List<NewsResourceDto>> GetSourcesAsync()
        {

            var sources = await _mediator.Send(new GetAllSourcesQuery());

            return sources;
        }

        public async Task<NewsResourceDto?> GetSourceIdsAsync(int id)
        {
            return _mapper.Map<NewsResourceDto>(await _unitOfWork.NewsResources.GetByIdAsync(id));
        }

        public async Task InitDefaultSourceAsync()
        {
            var isAnySourceNeedToBeInserted = false;
            if (!await IsSourceExistsAsync("Onliner"))
            {
                isAnySourceNeedToBeInserted = true;
                await _unitOfWork.NewsResources.AddAsync(new NewsResource() { Name = "Onliner", RssFeedUrl = "https://www.onliner.by/feed", OriginUrl = "https://www.onliner.by/" });
            }
            if (!await IsSourceExistsAsync("Admin"))
            {
                isAnySourceNeedToBeInserted = true;
                await _unitOfWork.NewsResources.AddAsync(new NewsResource() { Name = "Admin", RssFeedUrl = "Admin", OriginUrl = "Admin" });
                }
            if (!await IsSourceExistsAsync("Mail.ru"))
            {
                isAnySourceNeedToBeInserted = true;
                await _unitOfWork.NewsResources.AddAsync(new NewsResource() { Name = "Mail.ru", RssFeedUrl = "https://news.mail.ru/rss", OriginUrl = "https://news.mail.ru" });
            }
            if (!await IsSourceExistsAsync("Community"))
            {
                isAnySourceNeedToBeInserted = true;
                await _unitOfWork.NewsResources.AddAsync(new NewsResource() { Name = "Community", RssFeedUrl = "Community", OriginUrl = "Community" });
            }

            if (isAnySourceNeedToBeInserted)
            {
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> IsSourceExistsAsync(string name)
        {
            return await _unitOfWork.NewsResources
                .GetAsQueryable()
                .AnyAsync(s => s.Name.Equals(name));
        }

        public async Task<int> GetSourceIdByNameAsync(string source)
        {
            var s = await _unitOfWork.NewsResources.FindBy(s=>s.Name == source).FirstOrDefaultAsync();
            return s.Id;
        }
    }
}