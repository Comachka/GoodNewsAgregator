using myProject.Core.DTOs;

namespace myProject.Abstractions.Services
{
    public interface ICommentService
    {
        Task<List<CommentDto>> GetCommentsByArticleIdAsync(int articleId);
        Task<List<CommentDto>> CreateCommentAsync(CommentDto commentDto);
        Task<int> DeleteCommentsByIdAsync(int commentId);

    }
}