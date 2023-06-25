using myProject.Abstractions;
using myProject.Abstractions.Services;
using myProject.Core.DTOs;
using myProject.DataCQS.Commands;
using myProject.DataCQS.Queries;
using myProject.Data.Entities;
using AutoMapper;
using MediatR;
using Moq;
using Microsoft.EntityFrameworkCore;
using myProject.Repositories;
using System.ServiceModel.Syndication;
using System.Net;
using System.Threading;
using System.Xml;
using HtmlAgilityPack;
using System.Reflection;
using Moq.Protected;
using myProject.Business.RateModels;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace myProject.Business.Tests
{
    public class SubscriptionServiceTest
    {
        private readonly Mock<IUnitOfWork> _uowMock = new Mock<IUnitOfWork>();
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();


        private readonly Mock<ISubscriptionService> _subscriptionServiceMock = new Mock<ISubscriptionService>();

        private SubscriptionService CreateService()
        {

            var subscriptionService = new SubscriptionService(
                _uowMock.Object,
                _mapperMock.Object
                );

            return subscriptionService;
        }



        [Fact]
        public async Task GetSubscriptionsAsync_WhenSubscriptionExists_ReturnSubscriptionDto()
        {
            // Arrange
            var subscriptionService = CreateService();
            var subscriptions = new List<Subscription> { { new Subscription () } };
            _uowMock.Setup(u => u.Subscriptions.GetAsQueryable()).Returns(subscriptions.AsQueryable());
            _mapperMock.Setup(mapper => mapper.Map<SubscriptionDto>(It.IsAny<Subscription>()))
               .Returns(() => new SubscriptionDto());
            // Act
            var result = await subscriptionService.GetSubscriptionsAsync();
            // Assert
            Assert.IsType<List<SubscriptionDto>>(result);
        }

       
    }
}