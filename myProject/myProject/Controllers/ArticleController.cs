using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myProject.Data;

namespace myProject.Controllers
{
    public class ArticleController : Controller
    {
        private readonly MyProjectContext _dbContext;

        public ArticleController (MyProjectContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var data = await _dbContext.Categories.ToListAsync();
            return View(data);
        }
    }
}
