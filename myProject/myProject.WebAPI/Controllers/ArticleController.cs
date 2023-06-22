using Microsoft.AspNetCore.Mvc;
using myProject.WebAPI.Responses;
using myProject.WebAPI.Requests;
using myProject.Abstractions.Services;
using AutoMapper;
using Serilog;
using myProject.Core.DTOs;
using Microsoft.AspNetCore.Authorization;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace myProject.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleService _articleService;
        private readonly IMapper _mapper;
        private readonly ILogger<ArticleController> _logger;
        public ArticleController(IArticleService articleService,
            IMapper mapper,
            ILogger<ArticleController> logger)
        {
            _articleService = articleService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        //[Authorize]
        [ProducesResponseType(typeof(ArticleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromQuery] GetArticlesByPagingInfoRequest request)
        {
            try
            {
                var articles = (await _articleService.GetArticlesByPageAsync(request.Page, request.PageSize, request.MinRaiting))
                    .Select(dto => _mapper.Map<ArticleResponse>(dto));
                if (articles == null)
                {
                    return NotFound();
                }
                return Ok(articles);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, new ErrorResponse() { Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        //[Authorize]
        [ProducesResponseType(typeof(ArticleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var article = await _articleService.GetArticleByIdWithSourceNameAsync(id);
                if (article == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<ArticleResponse>(article));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, new ErrorResponse() { Message = ex.Message });
            }
        }

        [HttpPost]
        //[Authorize]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] CreateArticleRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Content))
                    ModelState.AddModelError("request.Content", "Content is null");

                if (string.IsNullOrWhiteSpace(request.Title))
                    ModelState.AddModelError("request.Title", "Title is null");

                if (string.IsNullOrWhiteSpace(request.ShortDescription))
                    ModelState.AddModelError("request.ShortDescription", "ShortDescription is null");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var articleDto = _mapper.Map<ArticleDto>(request);
                articleDto.DatePosting = DateTime.Now;
                await _articleService.AddAsync(articleDto);
                var id = await _articleService.GetIdOfArticleASync(articleDto);
                return Created($"/Article/{id}", null);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, new ErrorResponse() { Message = ex.Message });
            }
            
        }

        // Edit article
        [HttpPut("{id}")]
        //[Authorize]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Put([FromBody] UpdateArticleRequest request)
        {
            try
            {
                var articleDto = await _articleService.GetArticleByIdWithSourceNameAsync(request.Id);

                if (articleDto != null)
                {
                    await _articleService.EditArticleAsync(_mapper.Map<ArticleDto>(request));
                }
                else
                {
                    
                      return NotFound();
                    
                }
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, new ErrorResponse() { Message = ex.Message });
            }
            
        }

        // Delete article
        [HttpDelete("{id}")]
        //[Authorize]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _articleService.DeleteArticleByIdAsync(id);
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
