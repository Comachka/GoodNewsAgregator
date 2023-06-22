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
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountController> _logger;
        public AccountController(IUserService userService,
            IRoleService roleService,
            ISubscriptionService subscriptionService,
            IMapper mapper,
            ILogger<AccountController> logger)
        {
            _userService = userService;
            _roleService = roleService;
            _subscriptionService = subscriptionService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        //[Authorize]
        [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                var users = (await _userService.GetUsersAsync())
                .Select(dto => _mapper.Map<ProfileResponse>(dto));

                if (users == null)
                {
                    return NotFound();
                }

                foreach (var user in users)
                {
                    var role = await _roleService.GetUserRole(user.Id);
                    var subs = await _subscriptionService.GetMySubscriptionAsync(user.Id);
                    var onSubs = await _subscriptionService.GetOnMeSubscriptionAsync(user.Id);
                    user.OnMeLikes = onSubs.Count;
                    user.MyLikes = subs.Count;
                    user.Role = role;
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, new { Message = ex.Message });
            }
        }


        [HttpGet("{id}")]
        //[Authorize]
        [ProducesResponseType(typeof(ProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }
                var role = await _roleService.GetUserRole(id);
                var subs = await _subscriptionService.GetMySubscriptionAsync(id);
                var onSubs = await _subscriptionService.GetOnMeSubscriptionAsync(id);
                var profile = _mapper.Map<ProfileResponse>(user);
                profile.OnMeLikes = onSubs.Count;
                profile.MyLikes = subs.Count;
                profile.Role = role;

                return Ok(_mapper.Map<ProfileResponse>(profile));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, new ErrorResponse() { Message = ex.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterProfile([FromForm] RegisterProfileRequest request)
        {
            try
            {
                if (request.Avatar != null)
                {
                    string ext = Path.GetExtension(request.Avatar.FileName);
                    if ((request.Avatar.Length > 204800) || !((ext == ".png") || (ext == ".jpg") || (ext == ".jpeg")))
                    {
                        ModelState.AddModelError("request.AvatarChange", "The avatar must be in PNG or JPEG/JPG format up to 200kb in size");
                    }
                }
                if (await _userService.IsUserExistsAsync(request.Email))
                {
                    ModelState.AddModelError("request.Email", "This email already used");
                }

                if (string.IsNullOrWhiteSpace(request.AboutMyself))
                {
                    ModelState.AddModelError("request.AboutMyself", "AboutMyself not entered");
                }
                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    ModelState.AddModelError("request.Name", "Name not entered");
                }


                if (ModelState.IsValid)
                {
                    string modelAvatar = "";
                    if (request.Avatar == null)
                    {
                        modelAvatar = $"\\img\\Avatars\\low.jpg";
                    }
                    else
                    {
                        Directory.CreateDirectory($"{Environment.CurrentDirectory}\\wwwroot\\img\\Avatars\\{request.Email}\\");
                        var fileName = Path.GetFileName(request.Avatar.FileName);
                        var filePath = Path.Combine($"{Environment.CurrentDirectory}\\wwwroot\\img\\Avatars\\{request.Email}\\", fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await request.Avatar.CopyToAsync(fileStream);
                        }
                        modelAvatar = Path.Combine($"\\img\\Avatars\\{request.Email}", fileName);
                    }
                    try
                    {
                        var message = new MailMessage();
                        message.From = new MailAddress("evdokimchik2012@mail.ru");
                        message.To.Add(new MailAddress(request.Email));
                        message.Subject = "Mail notification";
                        message.Body = $"Hello {request.Name}!";
                        using (var client = new SmtpClient("smtp.mail.ru", 587))
                        {
                            client.UseDefaultCredentials = false;
                            client.Credentials = new NetworkCredential("evdokimchik2012@mail.ru", "rujDsf7qf1Q84LTF1Frc");
                            client.EnableSsl = true;
                            client.Send(message);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, ex.Message);
                        ModelState.AddModelError("request.Email", "Такого mail не существует");
                        return BadRequest();
                    }
                    var user = await _userService.RegisterAsync(request.Email,
                        request.Password,
                        request.AboutMyself,
                        request.Name,
                        request.MailNotification,
                        modelAvatar);

                    return Created($"/Profile/{user.Id}", null);
                }
                ModelState.AddModelError("", "Ошибка регистрации");
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, new ErrorResponse() { Message = ex.Message });
            }

        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        //[Authorize]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> EditProfile([FromForm] UpdateProfileRequest request)
        {
            try
            {

                if (request.AvatarChange != null)
                {
                    string ext = Path.GetExtension(request.AvatarChange.FileName);
                    if ((request.AvatarChange.Length > 204800) || !((ext == ".png") || (ext == ".jpg") || (ext == ".jpeg")))
                    {
                        ModelState.AddModelError("request.AvatarChange", "The avatar must be in PNG or JPEG/JPG format up to 200kb in size.");
                    }
                }

                if (ModelState.IsValid)
                {
                    var modelAvatar = "";
                    if (request.AvatarChange != null)
                    {
                        var fileName = Path.GetFileName(request.AvatarChange.FileName);
                        if (!Directory.Exists($"{Environment.CurrentDirectory}\\wwwroot\\img\\Avatars\\{request.Email}\\"))
                        {
                            Directory.CreateDirectory($"{Environment.CurrentDirectory}\\wwwroot\\img\\Avatars\\{request.Email}\\");
                        }
                        DirectoryInfo directoryInfo = new DirectoryInfo($"{Environment.CurrentDirectory}\\wwwroot\\img\\Avatars\\{request.Email}\\");
                        foreach (FileInfo file in directoryInfo.GetFiles())
                        {
                            file.Delete();
                        }
                        var filePath = Path.Combine($"{Environment.CurrentDirectory}\\wwwroot\\img\\Avatars\\{request.Email}\\", fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await request.AvatarChange.CopyToAsync(fileStream);
                        }
                        modelAvatar = Path.Combine($"\\img\\Avatars\\{request.Email}", fileName);
                    }
                    else
                    {
                        modelAvatar = "";
                    }

                    var user = await _userService.GetUserByEmailAsync(request.Email);
                    await _userService.ChangeProfileAsync(modelAvatar, request.AboutMyself, request.Name, request.MailNotification, user.Id);
                    return Ok();
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        //[Authorize]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _userService.DeleteUserByIdAsync(id);
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
