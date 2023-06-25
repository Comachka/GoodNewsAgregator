using myProject.Abstractions;
using myProject.Abstractions.Services;
using myProject.Core.DTOs;
using Microsoft.Extensions.Configuration;
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
    public class SourceServiceTest
    {
        private readonly Mock<IUnitOfWork> _uowMock = new Mock<IUnitOfWork>();
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();

        private readonly Mock<ISourceService> _courceServiceMock = new Mock<ISourceService>();

        private SourceService CreateService()
        {

            var sourceService = new SourceService(
                _uowMock.Object,
                _mapperMock.Object,
                _mediatorMock.Object,
                _configurationMock.Object
                );

            return sourceService;
        }

        [Theory]
        [InlineData(1)]
        [InlineData(21)]
        [InlineData(3)]
        public async Task GetSourceIdsAsync_CorrectId_ReturnNewsResourceDto(int userId)
        {
            // Arrange
            var sourceService = CreateService();
            _uowMock.Setup(u => u.NewsResources.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(() => new NewsResource());
            _mapperMock.Setup(mapper => mapper.Map<NewsResourceDto>(It.IsAny<NewsResource>()))
               .Returns(() => new NewsResourceDto());

            // Act
            var result = await sourceService.GetSourceIdsAsync(userId);

            // Assert
            Assert.IsType<NewsResourceDto>(result);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-21)]
        [InlineData(0)]
        public async Task GetSourceIdsAsync_IncorrectId_ThrowExeption(int userId)
        {
            // Arrange
            var sourceService = CreateService();
            _uowMock.Setup(u => u.NewsResources.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(() => null);
            _mapperMock.Setup(mapper => mapper.Map<NewsResourceDto>(It.IsAny<NewsResource>()))
               .Returns(() => new NewsResourceDto());

            // Act
            var result = async () => await sourceService.GetSourceIdsAsync(userId);

            // Assert
            await Assert.ThrowsAnyAsync<ArgumentException>(result);
        }


    }
}