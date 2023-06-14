using Microsoft.AspNetCore.Mvc;
using myProject.Models;
using myProject.Abstractions.Services;
using System.Security.Claims;
using myProject.Core.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Web;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using myProject.Business;
using AutoMapper;
using myProject.Data.Entities;
using System.Xml.Linq;
using Serilog;

namespace myProject.Mvc.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly IUserService _userService;
        private readonly IArticleService _articleService;
        private readonly IMapper _mapper;

        public CommentController(ICommentService commentService,
            IMapper mapper,
            IArticleService articleService,
            IUserService userService)
        {
            _commentService = commentService;
            _mapper = mapper;
            _userService = userService;
            _articleService = articleService;
        }

        
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CommentModel model)
        {
            var user = await _userService.GetUserByEmailAsync(User.Identity.Name);

            model.User = user.Name;
            model.UserId = user.Id;
            model.Avatar = user.Avatar;
            model.DateCreated = DateTime.Now;

            if (ModelState.IsValid)
            {
                var comments = await _commentService.CreateCommentAsync(_mapper.Map<CommentDto>(model));
                await _articleService.UpRaitingAsync(model.ArticleId);
                if (comments != null)
                {
                    // return Ok(comments);
                    return Ok(comments.FirstOrDefault(c => c.Content == model.Content));
                }
                ModelState.AddModelError("", "Cant create comment");
            }
            return Ok(false);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Super Moderator, Moderator")]
        public async Task<IActionResult> ManageComments(int id)
        {
            try
            {
                if (id == null)
                {
                    throw new Exception("Article id is null");
                }
                var dtos = await _commentService.GetCommentsByArticleIdAsync(id);
                var comments = dtos.Select(c => _mapper.Map<CommentModel>(c)).ToList();
                var model = new ManageCommentsModel()
                {
                    Comments = comments,
                    ArticleId = id
                };
                return View(model);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Super Moderator, Moderator")]
        public async Task<IActionResult> DeleteComment(int id)
        {
            try
            {
                if (id == null)
                {
                    throw new Exception("Id of deleting comment is null");
                }
                var articleId = await _commentService.DeleteCommentsByIdAsync(id);
                return RedirectToAction("ManageComments", "Comment", new { id = articleId });
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFakeComments(int articleId)
        {
            try
            {
                if (articleId == null)
                {
                    throw new Exception("Article id is null");
                }
                var comments = await _commentService.GetCommentsByArticleIdAsync(articleId);

                return Ok(comments);
            }
            catch(Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}

