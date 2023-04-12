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

        public async Task<IEnumerable<CategoryDto>> GetListCategoriesAsync()
        {
            return await _unitOfWork.Categories
                .GetAsQueryable()
                .Select(category => _mapper.Map<CategoryDto>(category))
                .ToListAsync();
        }

    }
}