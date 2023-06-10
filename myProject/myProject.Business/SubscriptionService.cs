using AutoMapper;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using myProject.Abstractions;
using myProject.Abstractions.Services;
using myProject.Business.RateModels;
using myProject.Core.DTOs;
using myProject.Data.Entities;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Text;
using System.Xml;
using System.Web;
using Azure;
using System;

namespace myProject.Business
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper; // Convert(article) => _mapper.Map<ArticleDto>(article);


        public SubscriptionService(IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<SubscriptionDto>> GetOnMeSubscriptionAsync(int myId)
        {
            var subs = await _unitOfWork.Subscriptions.FindBy(s => s.FollowOnId == myId)
                .Select(s => _mapper.Map<SubscriptionDto>(s))
                .ToListAsync();
            return subs;
        }
        public async Task<List<SubscriptionDto>> GetMySubscriptionAsync(int myId)
        {
            var subs = await _unitOfWork.Subscriptions.FindBy(s => s.FollowerId == myId)
                .Select(s => _mapper.Map<SubscriptionDto>(s))
                .ToListAsync();
            return subs;
        }
        public async Task DeleteSubscriptionByIdAsync(int myId, int subId)
        {
            var sub = await _unitOfWork.Subscriptions.FindBy(s => s.FollowerId == myId && s.FollowOnId == subId).FirstOrDefaultAsync();
            if (sub != null)
            {
                await _unitOfWork.Subscriptions.Remove(sub.Id);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task AddSubscriptionByIdAsync(int myId, int subId)
        {
            var sub = new Subscription
            {
                FollowerId = myId,
                FollowOnId = subId
            };

            await _unitOfWork.Subscriptions.AddAsync(sub);
            await _unitOfWork.SaveChangesAsync();
        }

    }
}