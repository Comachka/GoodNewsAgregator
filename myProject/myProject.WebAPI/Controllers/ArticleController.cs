using Microsoft.AspNetCore.Mvc;
using myProject.WebAPI.Responses;
using myProject.WebAPI.Requests;
using myProject.Abstractions.Services;
using AutoMapper;
using Serilog;
using myProject.Core.DTOs;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace myProject.WebAPI.Controllers
{
    /// <summary>
    /// Controller for work with Article resources
    /// </summary>
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

        /// <summary>
        /// Get Collection of articles by page size and page number
        /// </summary>
        /// <param name="request">Model which contains PageSize & Page number </param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ArticleResponse[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetArticlesByPagingInfoRequest request)
        {
            try
            {
                var articles = (await _articleService.GetArticlesByPageAsync(request.Page, request.PageSize, double.MinValue))
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

        // GET api/<ArticleController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var article = _articleService.GetArticleByIdWithSourceNameAsync(id);
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

        // POST api/<ArticleController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateOrUpdateArticleRequest request)
        {
            var articleDto = _mapper.Map<ArticleDto>(request);
            await _articleService.AddAsync(articleDto);
            //response should contain id of created resource
            return Created("/Article/1", null);
        }

        // PUT api/<ArticleController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] CreateOrUpdateArticleRequest request)
        {
            var articleDto = await _articleService.GetArticleByIdWithSourceNameAsync(request.Id);

            if (articleDto != null)
            {
                await _articleService.EditArticleAsync(_mapper.Map<ArticleDto>(request));
            }
            return Ok();
        }

        // DELETE api/<ArticleController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _articleService.DeleteArticleByIdAsync(id);
            return Ok();
        }
    }
}
