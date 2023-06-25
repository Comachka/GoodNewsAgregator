using myProject.Abstractions;
using myProject.Abstractions.Services;
using myProject.Core.DTOs;
using myProject.DataCQS.Commands;
using myProject.DataCQS.Queries;
using myProject.Data.Entities;
using AutoMapper;
using MediatR;
using Moq;
using Moq.Internals;
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
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace myProject.Business.Tests
{
    public class UserServiceTest
    {
        private readonly Mock<IUnitOfWork> _uowMock = new Mock<IUnitOfWork>();
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private readonly Mock<IRoleService> _roleServiceMock = new Mock<IRoleService>();
        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();

        private readonly Mock<IUserService> _userServiceMock = new Mock<IUserService>();

        private UserService CreateService()
        {
            var userService = new UserService(
                _uowMock.Object,
                _mapperMock.Object,
                _roleServiceMock.Object,
                _configurationMock.Object
                );

            return userService;
        }



        [Fact]
        public async Task ChangeRaiting_AdminUpRaiting_PathesOnyRaitingAdmin()
        {
            // Arrange

            var userService = CreateService();
            var userId = 1;
            var raiting = 100;
            var roles = new List<Role>
                {
                new Role { Id = 1, Name = "Администратор" },
                new Role { Id = 2, Name = "Пользователь" },
                new Role { Id = 3, Name = "Модератор" },
                new Role { Id = 4, Name = "Главный модератор" }
                };
            var user = new User { Id = 1, Raiting = 500, RoleId = 1 };
            _uowMock.Setup(u => u.Users.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(user);
            _uowMock.Setup(u => u.Roles.GetAsQueryable())
                .Returns(roles.AsQueryable());

            // Act
            await userService.ChangeRaiting(userId, raiting);

            // Assert
            _uowMock.Verify(u => u.Users.PatchAsync(userId, It.IsAny<List<PatchDto>>()), Times.Once());
        }

        [Fact]
        public async Task ChangeRaiting_UserUpRaitingToModer_PathesTwoTimes()
        {
            // Arrange
            var userService = CreateService();
            var userId = 1;
            var raiting = 10000;
            var user = new User { Id = userId, Raiting = 9000, RoleId = 2 };
            var roles = new List<Role>
                {
                new Role { Id = 1, Name = "Администратор" },
                new Role { Id = 2, Name = "Пользователь" },
                new Role { Id = 3, Name = "Модератор" },
                new Role { Id = 4, Name = "Главный модератор" }
                };
            _uowMock.Setup(u => u.Users.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(user);
            _uowMock.Setup(u => u.Roles.GetAsQueryable())
                .Returns(roles.AsQueryable());
            _uowMock.Setup(u => u.Users.PatchAsync(It.IsAny<int>(), It.IsAny<List<PatchDto>>()))
                .Callback<int, List<PatchDto>>((id, patches) =>
                {
                    patches.ForEach(patch =>
                    {
                        if (patch.PropertyName == nameof(UserDto.Raiting))
                        {
                            user.Raiting += raiting;
                            user.Raiting = (int)patch.PropertyValue;
                        }
                        else if (patch.PropertyName == nameof(UserDto.RoleId))
                        {
                            user.RoleId = (int)patch.PropertyValue;
                        }
                    });
                });

            // Act
            await userService.ChangeRaiting(userId, raiting);

            // Assert
            _uowMock.Verify(u => u.Users.PatchAsync(userId, It.IsAny<List<PatchDto>>()), Times.Exactly(2));
            Assert.Equal(19000, user.Raiting);
            Assert.Equal(3, user.RoleId);
        }

        [Fact]
        public async Task GetPasswordHash_WhenPasswordIsCorrect_ReturnsCorrectPasswordHash()
        {
            // Arrange
            var userService = CreateService();
            var password = "password";
            var passwordHash = "5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8";
            // Act
            var method = typeof(UserService).GetMethod("GetPasswordHash", BindingFlags.NonPublic | BindingFlags.Instance);
            var result = (string)method.Invoke(userService, new object[] { password });
            // Assert
            Assert.Equal(passwordHash, result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task GetUserByIdAsync_CorrectId_ReturnUser(int id)
        {
            // _
            var userService = CreateService();
            _uowMock.Setup(uow => uow.Users.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(() => new User());
            _mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>()))
                .Returns(() => new UserDto());


            // Act
            var result = await userService.GetUserByIdAsync(id);

            // Assert
            Assert.IsType<UserDto>(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-2)]
        [InlineData(-3)]
        public async Task GetUserByIdAsync_IncorrectId_ReturnNull(int id)
        {
            // _
            var userService = CreateService();
            _uowMock.Setup(uow => uow.Users.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(() => null);
            _mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>()))
                .Returns(() => new UserDto());


            // Act
            var result = await userService.GetUserByIdAsync(id);

            // Assert
            Assert.Equal(null, result);
        }


        [Fact]
        public async Task GetUserClamsAsync_CorrectUser_ReturnsCorrectClaims()
        {
            // Arrange
            var userService = CreateService();
            var user = new UserDto { Id = 1, Email = "test@test.com" };
            var role = "admin";
            var expectedClaimsCount = 2;

            _roleServiceMock.Setup(service => service.GetUserRole(user.Id)).ReturnsAsync(role);

            // Act
            var result = await userService.GetUserClamsAsync(user);

            // Assert
            Assert.IsType<List<Claim>>(result);
            Assert.Equal(expectedClaimsCount, result.Count);
            Assert.Contains(result, claim => claim.Type == ClaimsIdentity.DefaultNameClaimType && claim.Value == user.Email);
            Assert.Contains(result, claim => claim.Type == ClaimsIdentity.DefaultRoleClaimType && claim.Value == role);
        }

        [Fact]
        public async Task GetUserClamsAsync_UserRoleIsNullOrEmpty_ThrowsArgumentException()
        {
            // Arrange
            var userService = CreateService();
            var user = new UserDto { Id = 1, Email = "test@test.com" };
            string role = null;

            _roleServiceMock.Setup(service => service.GetUserRole(user.Id)).ReturnsAsync(role);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => userService.GetUserClamsAsync(user));
        }
    }
}