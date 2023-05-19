using AutoMapper;
using Microsoft.EntityFrameworkCore;
using myProject.Abstractions;
using myProject.Abstractions.Services;
using myProject.Core.DTOs;
using myProject.Data.Entities;

namespace myProject.Business
{
    public class ArticleService: IArticleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper; // Convert(article) => _mapper.Map<ArticleDto>(article);


        public ArticleService(IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<int> GetTotalArticlesCountAsync()
        {
            var count = await _unitOfWork.Articles.CountAsync();
            return count;
        }

        public async Task<IEnumerable<CategoryDto>> GetListCategoriesAsync()
        {
            return await _unitOfWork.Categories
                .GetAsQueryable()
                .Select(category => _mapper.Map<CategoryDto>(category))
                .ToListAsync();
        }

        public async Task<List<ArticleDto>> GetArticlesByPageAsync(int page, int pageSize)
        {
            try
            {
                var articles = (await _unitOfWork
                        .Articles
                        .GetArticlesForPageAsync(page, pageSize))
                    .Select(article => _mapper.Map<ArticleDto>(article))
                    .ToList();

                //var x = 0;
                //var z = 15 / x;

                return articles;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
        public async Task<ArticleDto?> GetArticleByIdWithSourceNameAsync(int id)
        {
            var article = await _unitOfWork.Articles.GetByIdAsync(id);
            var source = await _unitOfWork.NewsResources.GetByIdAsync(article.NewsResourceId);
            article.NewsResource = source;
            if (article != null)
            {
                var dto = _mapper.Map<ArticleDto>(article);
                dto.SourceName = source.Name;
                dto.ArticleSourceUrl = source.Link;
                return dto;
            }
            return null;
        }

        public async Task<List<AutoCompleteDataDto>> GetArticlesNamesByPartNameAsync(string partName)
        {
            var articles = await _unitOfWork.Articles
                .GetAsQueryable()
                .AsNoTracking()
                .Where(article => article.Title.Contains(partName))
                .Select(article => _mapper.Map<AutoCompleteDataDto>(article))
                .ToListAsync();

            return articles;

        }
    }
}