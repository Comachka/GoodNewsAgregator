using AutoMapper;
using Microsoft.EntityFrameworkCore;
using myProject.Abstractions;
using myProject.Abstractions.Services;
using myProject.Core.DTOs;
using myProject.Data.Entities;

namespace myProject.Business
{
    public class CommentService: ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper; // Convert(article) => _mapper.Map<ArticleDto>(article);


        public CommentService(IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<CommentDto>> GetCommentsByArticleIdAsync(int articleId)
        {
            var dtos = await _unitOfWork.Comments
                .FindBy(comment => comment.ArticleId == articleId)
                .Select(comment => _mapper.Map<CommentDto>(comment))
                .ToListAsync();
            foreach (var dto in dtos)
            {
                var user = await _unitOfWork.Users.GetByIdAsync(dto.UserId);
                dto.User = user.Name;
                dto.Avatar = user.Avatar;
            }
            return dtos;
        }

        public async Task<List<CommentDto>> CreateCommentAsync(CommentDto commentDto)
        {
            var commentEntry = await _unitOfWork.Comments.AddAsync(_mapper.Map<Comment>(commentDto));
            await _unitOfWork.SaveChangesAsync();
            var dtos = await GetCommentsByArticleIdAsync(commentEntry.Entity.ArticleId);
            return dtos;
        }

    }
}