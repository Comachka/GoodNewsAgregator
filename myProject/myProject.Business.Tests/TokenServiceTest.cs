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
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace myProject.Business.Tests
{
    public class TokenServiceTest
    {
        private readonly Mock<IMediator> _mediatorMock = new Mock<IMediator>();
        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();
        private readonly Mock<ILogger<TokenService>> _loggerMock = new Mock<ILogger<TokenService>>();


        private readonly Mock<ITokenService> _tokenServiceMock = new Mock<ITokenService>();

        private TokenService CreateService()
        {
            var tokenService = new TokenService(
                _mediatorMock.Object,
                _configurationMock.Object,
                _loggerMock.Object
                );

            return tokenService;
        }


        [Fact]
        public async Task GenerateJwtToken_ValidUser_ReturnsToken()
        {
            // Arrange
            var tokenServer = CreateService();
            var user = new UserDto { Id = 1, Email = "test@test.com" };
            var jwtToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9";

            _mediatorMock.Setup(m => m.Send(It.IsAny <GetUserRoleNameByUserIdQuery>(), new CancellationToken())).ReturnsAsync(() => "Admin");
            _configurationMock.Setup(x => x["Jwt:SecurityKey"]).Returns("0932CA69-8982-4EDF-8C3F-EB7D8CD8500B");
            _configurationMock.Setup(x => x["Jwt:Audience"]).Returns("https://localhost:7245/");
            _configurationMock.Setup(x => x["Jwt:Issuer"]).Returns("myProject.WebAPI");
            _configurationMock.Setup(x => x["Jwt:ExpireInMinutes"]).Returns("10");
            // Act
            var method = typeof(TokenService).GetMethod("GenerateJwtToken", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = await (Task<string>)method.Invoke(tokenServer, new object[] { user });

            // Assert
            Assert.Contains(jwtToken, result);
        }


    }
}