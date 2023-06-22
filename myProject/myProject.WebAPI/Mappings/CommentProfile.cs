using AutoMapper;
using myProject.Core.DTOs;
using myProject.Data.Entities;
using myProject.WebAPI.Requests;
using myProject.WebAPI.Responses;

namespace myProject.WebAPI.Mappings
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<CommentDto, CommentResponse>();
            CreateMap<CreateCommentRequest, CommentDto>();
            CreateMap<Comment, CommentDto>()
                .ForMember(ent => ent.User, opt => opt.Ignore());
            CreateMap<CommentDto, Comment>()
                .ForMember(dto => dto.User, opt => opt.Ignore());
        }
    }
}
