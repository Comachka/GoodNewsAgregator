using Microsoft.AspNetCore.Mvc;
using myProject.WebAPI.Responses;
using myProject.WebAPI.Requests;
using myProject.Abstractions.Services;
using AutoMapper;
using Serilog;
using myProject.Core.DTOs;
using Microsoft.AspNetCore.Authorization;
using myProject.Business;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net.Mail;
using System.Net;
using myProject.Data.Entities;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace myProject.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IArticleService _articleService;
        private readonly IMapper _mapper;
        private readonly ILogger<CommentController> _logger;
        public CommentController(ICommentService commentService,
            IArticleService articleService,
            IMapper mapper,
            ILogger<CommentController> logger)
        {
            _commentService = commentService;
            _articleService = articleService;
            _mapper = mapper;
            _logger = logger;
        }


        [HttpGet("{id}")]
        //[Authorize]
        [ProducesResponseType(typeof(CommentResponse), StatusCodes.Status200OK)]
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
                var comments = (await _commentService.GetCommentsByArticleIdAsync(id))
                .Select(dto => _mapper.Map<CommentResponse>(dto));

                if (comments == null)
                {
                    return NotFound();
                }

                return Ok(comments);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, new { Message = ex.Message });
            }
        }


        [HttpPost]
        //[Authorize]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] CreateCommentRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Content))
                    ModelState.AddModelError("request.Content", "Content is null");


                var model = _mapper.Map<CommentDto>(request);
                model.DateCreated = (DateTime.Now).ToString();

                if (ModelState.IsValid)
                {
                    var comments = await _commentService.CreateCommentAsync(model);
                    var id = comments.FirstOrDefault(c => ((c.DateCreated == model.DateCreated) && (c.UserId == model.UserId))).Id;
                    await _articleService.UpRaitingAsync(model.ArticleId);
                    return Created($"/Comment/{id}", null);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, new ErrorResponse() { Message = ex.Message });
            }

        }

        [HttpDelete("{id}")]
        //[Authorize]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _commentService.DeleteCommentsByIdAsync(id);
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
