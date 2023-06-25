using myProject.Abstractions;
using myProject.Abstractions.Services;
using myProject.Core.DTOs;
using myProject.Data.Entities;
using AutoMapper;
using Moq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using myProject.Repositories;
using Microsoft.Extensions.Configuration;

namespace myProject.Business.Tests
{
    public class CommentServiceTest
    {
        private readonly Mock<IUnitOfWork> _uowMock = new Mock<IUnitOfWork>();
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();


        private readonly Mock<ICommentService> _commentServiceMock = new Mock<ICommentService>();

        private CommentService CreateService()
        {

            var commentService = new CommentService(
                _uowMock.Object,
                _mapperMock.Object
                );

            return commentService;
        }



        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task GetCommentsByArticleIdAsync_WhenCorrectId_ReturnsComments(int articleId)
        {
            // Arrange
            var comments = new List<Comment> { new Comment { } };
            _mapperMock.Setup(mapper => mapper.Map<CommentDto>(It.IsAny<Comment>()))
               .Returns(() => new CommentDto());
            _uowMock.Setup(u => u.Comments.FindBy(com => com.ArticleId == It.IsAny<int>())).Returns(comments.AsQueryable());
            _uowMock.Setup(u => u.Users.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(() => new User { });
            var commentService = CreateService();
            // Act
            var result = await commentService.GetCommentsByArticleIdAsync(articleId);
            // Assert
            Assert.NotNull(result);
        }

    }
}