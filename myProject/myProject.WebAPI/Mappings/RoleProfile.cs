using AutoMapper;
using myProject.Core.DTOs;
using myProject.Data.Entities;
using myProject.WebAPI.Requests;
using myProject.WebAPI.Responses;

namespace myProject.WebAPI.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<RoleDto, RoleResponse>();
            CreateMap<RoleDto, Role>();
            CreateMap<Role, RoleDto>();
        }
    }
}
