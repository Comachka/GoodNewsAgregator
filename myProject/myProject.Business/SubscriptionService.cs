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
        private readonly IMapper _mapper;


        public SubscriptionService(IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<List<SubscriptionDto>> GetSubscriptionsAsync()
        {
            return await _unitOfWork.Subscriptions.GetAsQueryable().
                Select(s => _mapper.Map<SubscriptionDto>(s)).ToListAsync();
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
            if ((myId != null) && (subId != null))
            {
                var sub = await _unitOfWork.Subscriptions.FindBy(s => s.FollowerId == myId && s.FollowOnId == subId).FirstOrDefaultAsync();
                if (sub != null)
                {
                    await _unitOfWork.Subscriptions.Remove(sub.Id);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
            else
            {
                throw new Exception("Some of this users does not exists");
            }
        }

        public async Task AddSubscriptionByIdAsync(int myId, int subId)
        {
            if ((myId != null) && (subId != null))
            {
                var sub = new Subscription
                {
                    FollowerId = myId,
                    FollowOnId = subId
                };

                await _unitOfWork.Subscriptions.AddAsync(sub);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Some of this users does not exists");
            }
        }

    }
}