using AutoMapper;
using myProject.Core.DTOs;
using myProject.Data.Entities;
using myProject.WebAPI.Responses;

namespace myProject.WebAPI.Mappings
{
    public class SubscriptionProfile : Profile
    {
        public SubscriptionProfile()
        {
            CreateMap<Subscription, SubscriptionDto>();
            CreateMap<SubscriptionDto, Subscription>();
            CreateMap<SubscriptionDto, SubscribeResponse>();
        }
    }
}
