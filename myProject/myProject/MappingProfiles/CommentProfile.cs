using AutoMapper;
using myProject.Core.DTOs;
using myProject.Data.Entities;
using myProject.Models;

namespace myProject.Mvc.MappingProfiles
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, CommentDto>()
                .ForMember(ent => ent.User, opt => opt.Ignore());
            CreateMap<CommentDto, Comment>()
                .ForMember(dto => dto.User, opt => opt.Ignore());
            CreateMap<CommentDto, CommentModel>();
            CreateMap<CommentModel, CommentDto>();
        }
    }
}
