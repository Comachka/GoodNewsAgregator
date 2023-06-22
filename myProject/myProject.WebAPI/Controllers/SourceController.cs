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
    public class SourceController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly ISourceService _sourceService;
        private readonly IMapper _mapper;
        private readonly ILogger<SourceController> _logger;
        public SourceController(IUserService userService,
            IRoleService roleService,
            ISourceService sourceService,
            IMapper mapper,
            ILogger<SourceController> logger)
        {
            _userService = userService;
            _roleService = roleService;
            _sourceService = sourceService;
            _mapper = mapper;
            _logger = logger;
        }


        [HttpGet]
        [ProducesResponseType(typeof(SourceResponse[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var source = (await _sourceService.GetSourcesAsync())
                .Select(dto => _mapper.Map<SourceResponse>(dto));

                if (source == null)
                {
                    return NotFound();
                }

                return Ok(source);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, new { Message = ex.Message });
            }
        }

    }
}
