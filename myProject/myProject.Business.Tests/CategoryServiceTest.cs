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
using System.Data;

namespace myProject.Business.Tests
{
    public class CategoryServiceTest
    {
        private readonly Mock<IUnitOfWork> _uowMock = new Mock<IUnitOfWork>();
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();


        private readonly Mock<ICategoryService> _categoryServiceMock = new Mock<ICategoryService>();

        private CategoryService CreateService()
        {

            var categoryService = new CategoryService(
                _uowMock.Object,
                _mapperMock.Object
                );

            return categoryService;
        }


        [Fact]
        public async Task GetCategoriesAsync_WhenCategoriesExists_ReturnsCategories()
        {
            // Arrange
            var categorys = new List<Category> { new Category () };
            var categoryService = CreateService();
            _uowMock.Setup(u => u.Categories.GetAsQueryable()).Returns(categorys.AsQueryable());
            _mapperMock.Setup(mapper => mapper.Map<CategoryDto>(It.IsAny<Category>()))
               .Returns(() => new CategoryDto());

            // Act
            var result = await categoryService.GetCategoriesAsync();

            // Assert
            Assert.NotNull(result);
        }
    }
}