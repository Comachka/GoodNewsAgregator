using AutoMapper;
using myProject.Core.DTOs;
using myProject.Data.Entities;
using myProject.WebAPI.Requests;
using myProject.WebAPI.Responses;

namespace myProject.WebAPI.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, ProfileResponse>();
        }
    }
}
