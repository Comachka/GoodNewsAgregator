using myProject.Core.DTOs;

namespace myProject.Abstractions.Services
{
    public interface ICommentService
    {
        public Task<List<CommentDto>> GetCommentsByArticleIdAsync(int articleId);
        public Task<List<CommentDto>> CreateCommentAsync(CommentDto commentDto);


    }
}