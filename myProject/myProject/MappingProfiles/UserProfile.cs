using AutoMapper;
using myProject.Core.DTOs;
using myProject.Data.Entities;
using myProject.Models;

namespace myProject.Mvc.MappingProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<UserDto, MyAccountModel>();
            CreateMap<UserDto, ProfileModel>();
        }
    }
}
