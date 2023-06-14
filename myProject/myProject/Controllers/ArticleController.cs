using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myProject.Core.DTOs;
using myProject.Models;
using myProject.Abstractions.Services;
using Serilog;
using ILogger = Serilog.ILogger;
using AutoMapper;
using myProject.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace myProject.Controllers
{

    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ICommentService _commentService;
        private readonly IUserService _userService;
        private readonly ISourceService _sourceService;
        private readonly ICategoryService _categoryService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public ArticleController (IArticleService articleService,
            ICommentService commentService,
            IConfiguration configuration,
            IUserService userService,
            ISourceService sourceService,
            ICategoryService categoryService,
            IMapper mapper)
        {
            _articleService = articleService;
            _commentService = commentService;
            _userService = userService;
            _sourceService = sourceService;
            _categoryService = categoryService;
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
                        .GetArticlesByPageAsync(page, pageSize, 0.015);



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


        /// 
        [HttpGet]
        public async Task<IActionResult> Search(string searchString)
        {
            try
            {
                var totalArticlesCount = await _articleService.GetTotalArticlesCountAsync();
                if (int.TryParse(_configuration["Pagination:Articles:DefaultPageSize"], out var pageSize))
                {
                    var pageInfo = new PageInfo()
                    {
                        PageSize = pageSize,
                        PageNumber = 1,
                        TotalItems = totalArticlesCount
                    };

                    var articleDtos = await _articleService
                        .GetArticlesByPageAsync(1, pageSize, 0.015);

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
        /// 


        [HttpGet]
        [Authorize(Roles = "Admin, Super Moderator")]
        public async Task<IActionResult> CreateArticleWithSource()
        {
            var model = new CreateArticleWithSourceModel()
            {
                Categories = await _categoryService.GetCategoriesAsync()
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Super Moderator")]
        public async Task<IActionResult> CreateArticleWithSource(CreateArticleWithSourceModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.GetUserByEmailAsync(HttpContext.User.Identity.Name);
                var role = "";
                if (user.RoleId == 1)
                {
                    role = "Admin";
                }
                else
                {
                    role = "Super Moderator";
                }

                var articleDto = new ArticleDto()
                {
                    Title = model.Title,
                    ShortDescription = model.ShortDescription,
                    Content = model.Content,
                    PositiveRaiting = 0.016,
                    DatePosting = DateTime.Now,
                    NewsResourceId = 6,
                    ArticleSourceUrl = role,
                    SourceName = role,
                    CategoryId = model.CategoryId
                };
                await _articleService.AddAsync(articleDto);

                return RedirectToAction("Index", "Article");
            }
            ModelState.AddModelError("", "Article model is incorrect");
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageArticles(int page = 1)
        {
            try
            {
                var totalArticlesCount = await _articleService.GetTotalArticlesCountAsync();
                if (int.TryParse(_configuration["Pagination:Articles:DefaultPageSize"], out var pageSize))
                {
                    var pageInfo = new PageInfo()
                    {
                        PageSize = pageSize,
                        PageNumber = page,
                        TotalItems = totalArticlesCount
                    };

                    var articleDtos = await _articleService
                        .GetArticlesByPageAsync(page, pageSize, double.MinValue);

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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteArticles(int id)
        {
            try
            {
                await _articleService.DeleteArticleByIdAsync(id);
                return RedirectToAction("ManageArticles", "Article");
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
            try
            {
                if (id == null)
                {
                    throw new Exception("Article id is null");
                }

                var articleDto = await _articleService.GetArticleByIdWithSourceNameAsync(id);
                var user = await _userService.GetUserByEmailAsync(HttpContext.User.Identity.Name);
                if (articleDto != null)
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        var category = await _categoryService.GetCategoryByIdAsync(articleDto.CategoryId);
                        var model = new ArticleDetailsWithCreateCommentModel()
                        {
                            ArticleDetails = _mapper.Map<ArticleDetailsModel>(articleDto),
                            CreateComment = new CommentModel()
                            {
                                ArticleId = articleDto.Id
                            },
                            RoleId = user.RoleId
                        };
                        model.ArticleDetails.Category = category;
                        return View(model);
                    }
                    return RedirectToAction("Login", "Account");
                }
                return NotFound();
            }
            catch(Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, new { Message = ex.Message });
            }
            
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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Aggregator()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(ArticleDetailsWithCreateCommentModel article)
        {
            try
            {
                await _articleService.EditArticleAsync(_mapper.Map<ArticleDto>(article.ArticleDetails));
                return RedirectToAction("Details", "Article", new { id = article.ArticleDetails.Id });
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AggregateNews()
        {
            try
            {
                var sources = (await _sourceService.GetSourcesAsync())
                .Where(s => !string.IsNullOrEmpty(s.RssFeedUrl))
                .ToArray();

                if (sources == null)
                {
                    throw new Exception("Cant find any sources");
                }

                foreach (var sourceDto in sources)
                {
                    if (sourceDto.Name == "Admin")
                    {
                        continue;
                    }
                    var articlesDataFromRss = (await _articleService
                        .AggregateArticlesDataFromRssSourceAsync(sourceDto, CancellationToken.None));

                    var fullContentArticles = await _articleService.GetFullContentArticlesAsync(articlesDataFromRss);

                    await _articleService.AddArticlesAsync(fullContentArticles);
                }

                var unratedArticles = await _articleService.GetUnratedArticlesAsync();

                if (unratedArticles == null)
                {
                    throw new Exception("Cant find any unrated article after their creation");
                }

                foreach (var unratedArticle in unratedArticles)
                {
                    var rate = await _articleService.GetArticleRateAsync(unratedArticle.Id);
                    await _articleService.RateArticleAsync(unratedArticle.Id, rate);
                }

                return RedirectToAction("Index", "Article");
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}
