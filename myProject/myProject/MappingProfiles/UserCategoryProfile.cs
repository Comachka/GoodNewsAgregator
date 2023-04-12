using AutoMapper;
using myProject.Core.DTOs;
using myProject.Data.Entities;

namespace myProject.Mvc.MappingProfiles
{
    public class UserCategoryProfile : Profile
    {
        public UserCategoryProfile()
        {
            CreateMap<UserCategory, UserCategoryDto>();
            CreateMap<UserCategoryDto, UserCategory>();
        }
    }
}
