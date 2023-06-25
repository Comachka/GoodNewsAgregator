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
    public class RoleServiceTest
    {
        private readonly Mock<IUnitOfWork> _uowMock = new Mock<IUnitOfWork>();
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();


        private readonly Mock<IRoleService> _roleServiceMock = new Mock<IRoleService>();

        private RoleService CreateService()
        {

            var roleService = new RoleService(
                _uowMock.Object,
                _mapperMock.Object
                );

            return roleService;
        }



        [Theory]
        [InlineData(1)]
        [InlineData(21)]
        [InlineData(3)]
        public async Task GetUserRole_WhenUserExists_ReturnsUserRole(int userId)
        {
            // Arrange
            var roleService = CreateService();
            var user = new User { Id = userId, RoleId = 2 };
            var role = new Role { Id = 2, Name = "Admin" };
            _uowMock.Setup(u => u.Users.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(user);
            _uowMock.Setup(u => u.Roles.GetByIdAsync(user.RoleId)).ReturnsAsync(role);

            // Act
            var result = await roleService.GetUserRole(userId);

            // Assert
            Assert.Equal(role.Name, result);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(-3)]
        public async Task GetUserRole_WhenIncorrectId_ReturnsExeption(int userId)
        {
            // Arrange
            var roleService = CreateService();
            var user = new User { Id = userId, RoleId = 2 };
            var role = new Role { Id = 2, Name = "Admin" };
            _uowMock.Setup(u => u.Users.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(user);
            _uowMock.Setup(u => u.Roles.GetByIdAsync(user.RoleId)).ReturnsAsync(role);

            // Act
            var result = async () => await roleService.GetUserRole(userId);

            // Assert
            await Assert.ThrowsAnyAsync<ArgumentException>(result);
        }

        [Fact]
        public async Task GetUserRole_WhenUserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var roleService = CreateService();
            var userId = 1;
            _uowMock.Setup(u => u.Users.GetByIdAsync(userId)).ReturnsAsync(() => null);

            // Act
            var result = await roleService.GetUserRole(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserRole_WhenRoleDoesNotExist_ReturnsNull()
        {
            // Arrange
            var roleService = CreateService();
            var userId = 1;
            var roleId = 2;
            var user = new User { Id = userId, RoleId = roleId };
            _uowMock.Setup(u => u.Users.GetByIdAsync(userId)).ReturnsAsync(user);
            _uowMock.Setup(u => u.Roles.GetByIdAsync(roleId)).ReturnsAsync(() => null);

            // Act
            var result = await roleService.GetUserRole(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetRolesAsync_WhenRolesExist_ReturnRoles()
        {
            // Arrange
            var roleService = CreateService();
            var roles = new List<Role> { new Role { Id = 1, Name = "Admin" } };
            _uowMock.Setup(u => u.Roles.GetAsQueryable()).Returns(roles.AsQueryable());
            _mapperMock.Setup(mapper => mapper.Map<RoleDto>(It.IsAny<Role>()))
               .Returns(() => new RoleDto());

            // Act
            var result = await roleService.GetRolesAsync();

            // Assert
            Assert.NotNull(result);
        }


    }
}