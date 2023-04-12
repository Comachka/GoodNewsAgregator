using AutoMapper;
using myProject.Core.DTOs;
using myProject.Data.Entities;

namespace myProject.Mvc.MappingProfiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();
        }
    }
}
