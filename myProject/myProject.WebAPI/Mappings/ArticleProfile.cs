using AutoMapper;
using myProject.Core.DTOs;
using myProject.Data.Entities;
using myProject.WebAPI.Requests;
using myProject.WebAPI.Responses;

namespace myProject.WebAPI.Mappings
{
    public class ArticleProfile : Profile
    {
        public ArticleProfile()
        {
            CreateMap<Article, ArticleDto>()
                .ForMember(dto => dto.SourceName,
                opt
                    => opt.MapFrom(
                        article
                            => article.NewsResource.Name));
            CreateMap<ArticleDto, Article>();

            CreateMap<ArticleDto, ArticleResponse>();
            CreateMap<ArticleResponse, ArticleDto>();

            CreateMap<CreateArticleRequest, ArticleDto>();
            CreateMap<UpdateArticleRequest, ArticleDto>();


        }
    }
}
