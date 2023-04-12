using AutoMapper;
using myProject.Core.DTOs;
using myProject.Data.Entities;

namespace myProject.Mvc.MappingProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
        }
    }
}
