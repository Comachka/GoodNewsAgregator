﻿using AutoMapper;
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
        private readonly IMapper _mapper; 


        public CommentService(IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<List<CommentDto>> GetCommentsByArticleIdAsync(int articleId)
        {
            if (articleId >0)
            {
                var dtos = _unitOfWork.Comments
                               .FindBy(comment => comment.ArticleId == articleId)
                               .Select(comment => _mapper.Map<CommentDto>(comment))
                               .ToList();
                foreach (var dto in dtos)
                {
                    var user = await _unitOfWork.Users.GetByIdAsync(dto.UserId);
                    dto.User = user.Name;
                    dto.Avatar = user.Avatar;
                }
                return dtos;
            }
            else
            {
                throw new ArgumentException("Invalid article id");
            }
        }

        public async Task<List<CommentDto>> CreateCommentAsync(CommentDto commentDto)
        {
            var commentEntry = await _unitOfWork.Comments.AddAsync(_mapper.Map<Comment>(commentDto));
            await _unitOfWork.SaveChangesAsync();
            var dtos = await GetCommentsByArticleIdAsync(commentEntry.Entity.ArticleId);
            return dtos;
        }

        public async Task<int> DeleteCommentsByIdAsync(int commentId)
        {
            var comment = await _unitOfWork.Comments.GetByIdAsync(commentId);
            if (comment == null)
            {
                throw new Exception("Cant find comment by id");
            }
            await _unitOfWork.Comments.Remove(commentId);
            await _unitOfWork.SaveChangesAsync();
            return comment.ArticleId;
        }
    }
}