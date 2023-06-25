using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using myProject.Abstractions;
using myProject.Abstractions.Services;
using myProject.Core.DTOs;
using myProject.Data.Entities;
using System.Text;
using System.Security.Cryptography;


namespace myProject.Business
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        private async Task<bool> IsCategoryExistsAsync(string name)
        {
            return await _unitOfWork.Categories
                .GetAsQueryable()
                .AnyAsync(cat => cat.Name.Equals(name));
        }

        public async Task InitiateDefaultCategorysAsync()
        {
            var isAnyCategoryNeedToBeInserted = false;
            if (!await IsCategoryExistsAsync("Культура"))
            {
                isAnyCategoryNeedToBeInserted = true;
                await _unitOfWork.Categories.AddAsync(new Category() { Name = "Культура" });
            }
            if (!await IsCategoryExistsAsync("Наука и технологии"))
            {
                isAnyCategoryNeedToBeInserted = true;
                await _unitOfWork.Categories.AddAsync(new Category() { Name = "Наука и технологии" });
            }
            if (!await IsCategoryExistsAsync("Политика"))
            {
                isAnyCategoryNeedToBeInserted = true;
                await _unitOfWork.Categories.AddAsync(new Category() { Name = "Политика" });
            }
            if (!await IsCategoryExistsAsync("Спорт"))
            {
                isAnyCategoryNeedToBeInserted = true;
                await _unitOfWork.Categories.AddAsync(new Category() { Name = "Спорт" });
            }
            if (!await IsCategoryExistsAsync("Экономика"))
            {
                isAnyCategoryNeedToBeInserted = true;
                await _unitOfWork.Categories.AddAsync(new Category() { Name = "Экономика" });
            }
            if (!await IsCategoryExistsAsync("Общество"))
            {
                isAnyCategoryNeedToBeInserted = true;
                await _unitOfWork.Categories.AddAsync(new Category() { Name = "Общество" });
            }
            if (!await IsCategoryExistsAsync("Разное"))
            {
                isAnyCategoryNeedToBeInserted = true;
                await _unitOfWork.Categories.AddAsync(new Category() { Name = "Разное" });
            }

            if (isAnyCategoryNeedToBeInserted)
            {
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            return _unitOfWork.Categories.GetAsQueryable().
                Select(category => _mapper.Map<CategoryDto>(category)).ToList();
        }

        public async Task<string> GetCategoryByIdAsync(int id)
        {
            if (id > 0)
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(id);
                return category.Name;
            }
            throw new ArgumentException("Invalid page or pageSize");
        }
    }
}
