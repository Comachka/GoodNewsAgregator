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
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger,
            IArticleService articleService,
            IMapper mapper)
        {
            _logger = logger;
            _articleService = articleService;
            _mapper = mapper;
        }

        //[Route("index")]
        //[MyCustomActionFilter]
        public async Task<IActionResult> Index()
        {
            var favArticles = (await _articleService
                .GetArticlesByPageAsync(1, 3)).Select(dto => _mapper.Map<ArticlePreviewModel>(dto))
                .ToList();

            var model = new HomePageModel()
            {
                FavouredArticles = favArticles
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