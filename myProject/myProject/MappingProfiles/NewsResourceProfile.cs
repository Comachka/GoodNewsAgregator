using AutoMapper;
using myProject.Core.DTOs;
using myProject.Data.Entities;

namespace myProject.Mvc.MappingProfiles
{
    public class NewsResourceProfile : Profile
    {
        public NewsResourceProfile()
        {
            CreateMap<NewsResource, NewsResourceDto>();
            CreateMap<NewsResourceDto, NewsResource>();
        }
    }
}
