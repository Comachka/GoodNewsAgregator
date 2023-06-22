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
    public class SubscribeController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IMapper _mapper;
        private readonly ILogger<SubscribeController> _logger;
        public SubscribeController(IUserService userService,
            IRoleService roleService,
            ISubscriptionService subscriptionService,
            IMapper mapper,
            ILogger<SubscribeController> logger)
        {
            _userService = userService;
            _roleService = roleService;
            _subscriptionService = subscriptionService;
            _mapper = mapper;
            _logger = logger;
        }


        [HttpGet]
        [ProducesResponseType(typeof(SubscribeResponse[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var subs = (await _subscriptionService.GetSubscriptionsAsync())
                .Select(dto => _mapper.Map<SubscribeResponse>(dto));

                if (subs == null)
                {
                    return NotFound();
                }

                return Ok(subs);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, new { Message = ex.Message });
            }
        }


        //Subcribe
        [HttpPost]
        //[Authorize]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Subscribe([FromQuery] SubscribeRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                var user = await _userService.GetUserByIdAsync(request.UserId);
                var sub = await _userService.GetUserByIdAsync(request.SubscribeId);
                if ((user == null) || (sub == null))
                {
                    return BadRequest(ModelState);
                }

                var role = await _roleService.GetUserRole(user.Id);
                var subs = await _subscriptionService.GetMySubscriptionAsync(user.Id);
                bool amISub = subs.Any(sub => sub.FollowerId == user.Id);

                if (!amISub)
                {
                    await _subscriptionService.AddSubscriptionByIdAsync(user.Id, request.SubscribeId);
                    switch (role)
                    {
                        case "Администратор":
                            await _userService.ChangeRaiting(request.SubscribeId, 10);
                            break;
                        case "Модератор":
                            await _userService.ChangeRaiting(request.SubscribeId, 3);
                            break;
                        case "Главный модератор":
                            await _userService.ChangeRaiting(request.SubscribeId, 5);
                            break;
                        default:
                            await _userService.ChangeRaiting(request.SubscribeId, 1);
                            break;
                    }
                }
                else
                {
                    await _subscriptionService.DeleteSubscriptionByIdAsync(user.Id, request.SubscribeId);
                    switch (role)
                    {
                        case "Администратор":
                            await _userService.ChangeRaiting(request.SubscribeId, -10);
                            break;
                        case "Модератор":
                            await _userService.ChangeRaiting(request.SubscribeId, -3);
                            break;
                        case "Главный модератор":
                            await _userService.ChangeRaiting(request.SubscribeId, -5);
                            break;
                        default:
                            await _userService.ChangeRaiting(request.SubscribeId, -1);
                            break;
                    }
                }
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
