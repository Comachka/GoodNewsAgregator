using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myProject.Core.DTOs;
using myProject.Abstractions.Services;

namespace myProject.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;

        public ArticleController (IArticleService articleService)
        {
            _articleService = articleService;
        }

       [HttpGet]
        public async Task<IActionResult> Index()
        {
            var data = await _articleService.GetListCategoriesAsync();
            return View(data);
        }
    }
}
