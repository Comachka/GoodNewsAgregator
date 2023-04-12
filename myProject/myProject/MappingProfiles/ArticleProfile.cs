using AutoMapper;
using myProject.Core.DTOs;
using myProject.Data.Entities;

namespace myProject.Mvc.MappingProfiles
{
    public class ArticleProfile : Profile
    {
        public ArticleProfile()
        {
            CreateMap<Article, ArticleDto>();
            CreateMap<ArticleDto, Article>();
        }
    }
}
