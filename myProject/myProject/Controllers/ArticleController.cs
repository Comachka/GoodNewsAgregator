using Microsoft.AspNetCore.Mvc;
using myProject.Core.DTOs;
using myProject.Models;
using myProject.Abstractions.Services;
using Serilog;
using ILogger = Serilog.ILogger;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Hangfire;

namespace myProject.Controllers
{

    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly IUserService _userService;
        private readonly ISourceService _sourceService;
        private readonly ICategoryService _categoryService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly ILogger<ArticleController> _logger;
        private readonly IRoleService _roleService;

        public ArticleController (IArticleService articleService,
            IConfiguration configuration,
            IUserService userService,
            ISourceService sourceService,
            ICategoryService categoryService,
            IRoleService roleService,
            IMapper mapper,
            ILogger<ArticleController> logger)
        {
            _articleService = articleService;
            _userService = userService;
            _sourceService = sourceService;
            _categoryService = categoryService;
            _configuration = configuration;
            _mapper = mapper;
            _logger = logger;
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                var totalArticlesCount = await _articleService.GetTotalArticlesCountAsync(0.015);
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
                    var roleName = "";
                    if (User.Identity.IsAuthenticated)
                    {
                        var user = await _userService.GetUserByEmailAsync(HttpContext.User.Identity.Name);
                        roleName = await _roleService.GetUserRole(user.Id);
                    }
                    else
                    {
                        roleName = "Аноним";
                    }
                    
                    return View(new ArticlesWithPaginationModel()
                    {
                        ArticlePreviews = articles,
                        PageInfo = pageInfo,
                        Role = roleName
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
                var totalArticlesCount = await _articleService.GetTotalArticlesCountAsync(0.015);
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
        [Authorize(Roles = "Администратор, Главный модератор")]
        public async Task<IActionResult> CreateArticleWithSource()
        {
            await _categoryService.InitiateDefaultCategorysAsync();
            var model = new CreateArticleWithSourceModel()
            {
                Categories = await _categoryService.GetCategoriesAsync()
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Администратор, Главный модератор")]
        public async Task<IActionResult> CreateArticleWithSource(CreateArticleWithSourceModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userService.GetUserByEmailAsync(HttpContext.User.Identity.Name);
                var role = await _roleService.GetUserRole(user.Id);
                if (role == "Администратор")
                {
                    role = "Admin";
                }
                else
                {
                    role = "Community";
                }

                await _sourceService.InitDefaultSourceAsync();
                var sourceId = await _sourceService.GetSourceIdByNameAsync(role);

                var articleDto = new ArticleDto()
                {
                    Title = model.Title,
                    ShortDescription = model.ShortDescription,
                    Content = model.Content,
                    PositiveRaiting = 0.016,
                    DatePosting = DateTime.Now,
                    NewsResourceId = sourceId,
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
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> ManageArticles(int page = 1)
        {
            try
            {
                var totalArticlesCount = await _articleService.GetTotalArticlesCountAsync(double.MinValue);
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
        [Authorize(Roles = "Администратор")]
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
                        var roleName = await _roleService.GetUserRole(user.Id);
                        var model = new ArticleDetailsWithCreateCommentModel()
                        {
                            ArticleDetails = _mapper.Map<ArticleDetailsModel>(articleDto),
                            CreateComment = new CommentModel()
                            {
                                ArticleId = articleDto.Id
                            },
                            Role = roleName
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
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Aggregator()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Администратор, Главный модератор")]
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
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> AggregateNews()
        {
            try
            {
                RecurringJob.AddOrUpdate(
                    "GetAllArticleDataMVC",
                    () => _articleService.AggregateArticlesDataFromRssAsync(new CancellationToken()),
                    "0,20,40 * * * *");

                RecurringJob.AddOrUpdate(
                    "UpdateArticleTextMVC",
                    () => _articleService.AddFullContentForArticlesAsync(new CancellationToken()),
                    "5,30 * * * *");

                RecurringJob.AddOrUpdate(
                    "AddArticleRaitingMVC",
                    () => _articleService.AddRaitingForArticlesAsync(new CancellationToken()),
                    "10,35 * * * *");

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
