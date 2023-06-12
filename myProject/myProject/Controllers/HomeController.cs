using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using myProject.Abstractions.Services;
using myProject.Models;
using System.Diagnostics;

namespace myProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IArticleService _articleService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger,
            IArticleService articleService,
            ICategoryService categoryService,
            IMapper mapper)
        {
            _logger = logger;
            _articleService = articleService;
            _categoryService = categoryService;
            _mapper = mapper;
        }

        //[Route("index")]
        //[MyCustomActionFilter]
        public async Task<IActionResult> Index()
        {
            var favArticles = await _articleService.GetFavArticleAsync();
            var favModel = favArticles.Select(dto => _mapper.Map<ArticlePreviewModel>(dto))
                .ToList();

            foreach (var article in favArticles)
            {
                foreach (var artModel in favModel)
                {
                    var category = await _categoryService.GetCategoryByIdAsync(article.CategoryId);
                    artModel.Category = category;
                }
            }
            
            var model = new HomePageModel()
            {
                FavouredArticles = favModel
            };

            return View(model);
        }

        //[MyCustomActionFilter]
        [Route("Privacy/general")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Route("error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
    }
}