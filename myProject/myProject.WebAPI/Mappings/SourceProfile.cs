using AutoMapper;
using myProject.Core.DTOs;
using myProject.Data.Entities;
using myProject.WebAPI.Requests;
using myProject.WebAPI.Responses;

namespace myProject.WebAPI.Mappings
{
        public class SourceProfile : Profile
        {
            public SourceProfile()
            {
                CreateMap<NewsResource, NewsResourceDto>();
                CreateMap<NewsResourceDto, NewsResource>();
                CreateMap<NewsResourceDto, SourceResponse>();
        }
        }
    
}
