using AutoMapper;
using myProject.Core.DTOs;
using myProject.Data.Entities;
using myProject.Models;

namespace myProject.Mvc.MappingProfiles
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
                //.ForMember(dto => dto.ArticleSourceUrl,
                //opt
                //    => opt.MapFrom(
                //        article
                //            => article.NewsResource.OriginUrl));
            
            CreateMap<ArticleDto, Article>();
            CreateMap<ArticleDto, ArticlePreviewModel>();
            CreateMap<ArticleDto, ArticleDetailsModel>();
            CreateMap<ArticleDetailsModel, ArticleDto>();
            CreateMap<Article, AutoCompleteDataDto>()
            .ForMember(dto => dto.Label,
                opt
                    => opt.MapFrom(
                        article
                            => article.Title))
            .ForMember(dto => dto.Value, 
            opt 
                => opt.MapFrom(
                     article
                        => article.Id)); //source -> destination

        }
    }
}
