using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myProject.Core.DTOs;
using myProject.Models;
using myProject.Abstractions.Services;
using Serilog;
using ILogger = Serilog.ILogger;
using AutoMapper;
using myProject.Data.Entities;

namespace myProject.Controllers
{

    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ICommentService _commentService;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public ArticleController (IArticleService articleService,
            ICommentService commentService,
            IConfiguration configuration,
            IUserService userService,
            IMapper mapper)
        {
            _articleService = articleService;
            _commentService = commentService;
            _userService = userService;
            _configuration = configuration;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            //Log.Information("Hello there");
            try
            {
                var totalArticlesCount = await _articleService.GetTotalArticlesCountAsync();
                //Log.Debug("Count of articles was gotten successfully");
                if (int.TryParse(_configuration["Pagination:Articles:DefaultPageSize"], out var pageSize))
                {
                    var pageInfo = new PageInfo()
                    {
                        PageSize = pageSize,
                        PageNumber = page,
                        TotalItems = totalArticlesCount
                    };

                    var articleDtos = await _articleService
                        .GetArticlesByPageAsync(page, pageSize);

                    var articles = articleDtos
                        .Select(dto =>
                            _mapper.Map<ArticlePreviewModel>(dto))
                        .ToList();

                    return View(new ArticlesWithPaginationModel()
                    {
                        ArticlePreviews = articles,
                        PageInfo = pageInfo
                    });
                }

                else
                {
                    Log.Warning("Trying to get page with incorrect pageNumber");
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var articleDto = await _articleService.GetArticleByIdWithSourceNameAsync(id);

            if (articleDto != null)
            {
                //var comments = await _commentService.GetCommentsByArticleIdAsync(articleDto.Id);
                if (User.Identity.IsAuthenticated)
                {
                    //var user = await _userService.GetUserByEmailAsync(User.Identity.Name);
                    var model = new ArticleDetailsWithCreateCommentModel()
                    {
                        ArticleDetails = new ArticleDetailsModel()
                        {
                            Id = articleDto.Id,
                            Title = articleDto.Title,
                            PositiveRaiting = articleDto.PositiveRaiting,
                            SourceName = articleDto.SourceName,
                            Content = articleDto.Content
                        },
                        //Comments = comments.Select(dto => _mapper.Map<CommentModel>(dto)).ToList(),
                        CreateComment = new CommentModel()
                        {
                            ArticleId = articleDto.Id
                            //User = user.Name,
                            //UserId = user.Id,
                            //Avatar = user.Avatar
                        }

                    };
                    return View(model);
                }
                return RedirectToAction("Login", "Account");
                
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetArticlesNames(string name = "")
        {
            try
            {
                var names = await _articleService.GetArticlesNamesByPartNameAsync(name);
                return Ok(names);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}
