using Microsoft.AspNetCore.Mvc;
using myProject.WebAPI.Responses;
using myProject.WebAPI.Requests;
using myProject.Abstractions.Services;
using Hangfire;
using AutoMapper;
using Serilog;
using myProject.Core.DTOs;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace myProject.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AggregtorController : ControllerBase
    {
        private readonly IArticleService _articleService;

        public AggregtorController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Init()
        {
            try
            {
                RecurringJob.AddOrUpdate(
                "GetAllArticleData",
                () => _articleService.AggregateArticlesDataFromRssAsync(new CancellationToken()),
                "0,20,40 * * * *");

                RecurringJob.AddOrUpdate(
                    "UpdateArticleText",
                    () => _articleService.AddFullContentForArticlesAsync(new CancellationToken()),
                    "5,30 * * * *");

                RecurringJob.AddOrUpdate(
                    "AddArticleRaiting",
                    () => _articleService.AddRaitingForArticlesAsync(new CancellationToken()),
                    "10,35 * * * *");

                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, new ErrorResponse() { Message = ex.Message });
            }
        }

    }
}
