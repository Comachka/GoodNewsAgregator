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
                ModelState.AddModelError("", "Smth goes wrong");
            }
            return Ok(false);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageComments()
        {
            return Ok(/*await _userService.GetUsersAsync()*/);
        }

        [HttpGet]
        public async Task<IActionResult> GetFakeComments(int articleId)
        {
            var comments = await _commentService.GetCommentsByArticleIdAsync(articleId);

            return Ok(comments);
        }

    }
}

