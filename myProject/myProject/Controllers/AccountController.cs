using Microsoft.AspNetCore.Mvc;
using myProject.Models;
using myProject.Abstractions.Services;
using System.Security.Claims;
using myProject.Core.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using AutoMapper;
using Serilog;
using System.Net.Mail;
using System.Net;

namespace myProject.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ISubscriptionService _subscriptionService;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUserService userService,
            IRoleService roleService,
            ISubscriptionService subscriptionService,
            IMapper mapper,
            ILogger<AccountController> logger)
        {
            _userService = userService;
            _subscriptionService = subscriptionService;
            _roleService = roleService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> ChangeProfile(MyAccountModel model)
        {
            try
            {
                var errors = ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .Select(x => new { x.Key, x.Value.Errors })
            .ToArray();

                if (ModelState.IsValid)
                {
                    var modelAvatar = "";
                    if (model.AvatarChange != null)
                    {
                        var fileName = Path.GetFileName(model.AvatarChange.FileName);
                        if (!Directory.Exists($"{Environment.CurrentDirectory}\\wwwroot\\img\\Avatars\\{HttpContext.User.Identity.Name}\\"))
                        {
                            Directory.CreateDirectory($"{Environment.CurrentDirectory}\\wwwroot\\img\\Avatars\\{HttpContext.User.Identity.Name}\\");
                        }
                        DirectoryInfo directoryInfo = new DirectoryInfo($"{Environment.CurrentDirectory}\\wwwroot\\img\\Avatars\\{HttpContext.User.Identity.Name}\\");
                        foreach (FileInfo file in directoryInfo.GetFiles())
                        {
                            file.Delete();
                        }
                        var filePath = Path.Combine($"{Environment.CurrentDirectory}\\wwwroot\\img\\Avatars\\{HttpContext.User.Identity.Name}\\", fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.AvatarChange.CopyToAsync(fileStream);
                        }
                        modelAvatar = Path.Combine($"\\img\\Avatars\\{HttpContext.User.Identity.Name}", fileName);
                    }
                    else
                    {
                        modelAvatar = "";
                    }

                    var user = await _userService.GetUserByEmailAsync(HttpContext.User.Identity.Name);
                    await _userService.ChangeProfileAsync(modelAvatar, model.AboutMyself, model.Name, model.MailNotification, user.Id);
                    return RedirectToAction("MyAccount", "Account");
                }
                if (!(ModelState.GetFieldValidationState("AvatarChange") == ModelValidationState.Valid))
                {
                    TempData["ErrorMessage"] = "Пожалуйства выберите аватар формата PNG или JPEG/JPG размером до 200кб";
                }
                return RedirectToAction("MyAccount", "Account", model);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                if (id == null)
                {
                    throw new Exception("User id is null");
                }
                await _userService.DeleteUserByIdAsync(id);
                return RedirectToAction("ManageUsers", "Account");
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("MyAccount", "Account");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {

            var errors = ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .Select(x => new { x.Key, x.Value.Errors })
            .ToArray();
            if (ModelState.IsValid)
            {
                string modelAvatar = "";
                if (model.Avatar == null)
                {
                    modelAvatar = $"\\img\\Avatars\\low.jpg";
                }
                else
                {
                    Directory.CreateDirectory($"{Environment.CurrentDirectory}\\wwwroot\\img\\Avatars\\{model.Email}\\");
                    var fileName = Path.GetFileName(model.Avatar.FileName);
                    var filePath = Path.Combine($"{Environment.CurrentDirectory}\\wwwroot\\img\\Avatars\\{model.Email}\\", fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Avatar.CopyToAsync(fileStream);
                    }
                    modelAvatar = Path.Combine($"\\img\\Avatars\\{model.Email}", fileName);
                }
                    try
                    {
                        var message = new MailMessage();
                        message.From = new MailAddress("evdokimchik2012@mail.ru");
                        message.To.Add(new MailAddress(model.Email));
                        message.Subject = "Mail notification";
                        message.Body = $"Hello {model.Name}!";
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
                        ModelState.AddModelError("Email", "Такого mail не существует.");
                        ModelState.AddModelError("", "Ошибка регистрации");
                        return View(model);
                    }
                var user = await _userService.RegisterAsync(model.Email,
                    model.Password,
                    model.AboutMyself,
                    model.Name,
                    model.MailNotification,
                    modelAvatar);
                if (user != null)
                {
                    await AuthenticateAsync(user);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Ошибка регистрации");
            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Logout([FromQuery] string? returnUrl)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Login([FromQuery] string? returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("MyAccount", "Account");
            }
            if (!string.IsNullOrEmpty(returnUrl))
            {
                var lm = new LoginModel()
                {
                    ReturnUrl = returnUrl
                };
                return View(lm);
            }
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Subscribe(ProfileModel profile, [FromQuery] string? returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    var user = await _userService.GetUserByEmailAsync(HttpContext.User.Identity.Name);
                    var role = await _roleService.GetUserRole(user.Id);
                    if (!profile.Subscribe)
                    {
                        await _subscriptionService.AddSubscriptionByIdAsync(user.Id, profile.Id);
                        switch (role)
                        {
                            case "Администратор":
                                await _userService.ChangeRaiting(profile.Id, 10);
                                break;
                            case "Модератор":
                                await _userService.ChangeRaiting(profile.Id, 3);
                                break;
                            case "Главный модератор":
                                await _userService.ChangeRaiting(profile.Id, 5);
                                break;
                            default:
                                await _userService.ChangeRaiting(profile.Id, 1);
                                break;
                        }
                    }
                    else
                    {
                        await _subscriptionService.DeleteSubscriptionByIdAsync(user.Id, profile.Id);
                        switch (role)
                        {
                            case "Администратор":
                                await _userService.ChangeRaiting(profile.Id, -10);
                                break;
                            case "Модератор":
                                await _userService.ChangeRaiting(profile.Id, -3);
                                break;
                            case "Главный модератор":
                                await _userService.ChangeRaiting(profile.Id, -5);
                                break;
                            default:
                                await _userService.ChangeRaiting(profile.Id, -1);
                                break;
                        }
                    }
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("MyAccount", "Account");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, ex.Message);
                    return StatusCode(500, new { Message = ex.Message });
                }
            }
            return RedirectToAction("Login", "Account");
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            TempData["ErrorMessage"] = null;
            if (await _userService.IsUserExistsAsync(model.Email)
                && await _userService.IsPasswordCorrectAsync(model.Email, model.Password))
            {
                var user = await _userService.GetUserByEmailAsync(model.Email);
                await AuthenticateAsync(user);

                if (!string.IsNullOrEmpty(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                return RedirectToAction("MyAccount", "Account");
            }
                TempData["ErrorMessage"] = "Почта либо пароль указаны неверно";
                return RedirectToAction("Login", "Account", new { returnUrl = model.ReturnUrl });
        }

        [HttpGet]
        public async Task<IActionResult> CheckIsUserEmailIsValidAndNotExists(string email)
        {
            return Ok(!await _userService.IsUserExistsAsync(email));
        }

        [HttpGet]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> ManageUsers()
        {
            var users = await _userService.GetUsersAsync();
            var admin = users.FirstOrDefault(u => u.Email == HttpContext.User.Identity.Name);
            users.Remove(admin);
            
            var model = new ManageUsersModel
            {
                Users = users.Select(user => _mapper.Map<ProfileModel>(user)).OrderByDescending(u => u.Raiting).ToList()
            };
            foreach (var user in model.Users)
            {
                var role = await _roleService.GetUserRole(user.Id);
                user.Role = role;
            }
            return View(model);
        }
        
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyAccount()
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(HttpContext.User.Identity.Name);
                var role = await _roleService.GetUserRole(user.Id);
                if (user != null)
                {
                    var subs = await _subscriptionService.GetMySubscriptionAsync(user.Id);
                    var onSubs = await _subscriptionService.GetOnMeSubscriptionAsync(user.Id);
                    var profile = _mapper.Map<MyAccountModel>(user);
                    profile.OnMeLikes = onSubs.Count;
                    profile.MyLikes = subs.Count;
                    profile.Role = role;
                    return View(profile);
                }
                else
                {
                    throw new Exception("Identification error");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile(int id)
        {
            try
            {
                if (id == null)
                {
                    throw new Exception("User id is null");
                }
                var user = await _userService.GetUserByIdAsync(id);
                if (user.Email == HttpContext.User.Identity.Name) return RedirectToAction("MyAccount", "Account");
                if (user != null)
                {
                    var me = await _userService.GetUserByEmailAsync(HttpContext.User.Identity.Name);
                    var role = await _roleService.GetUserRole(user.Id);
                    var subs = await _subscriptionService.GetMySubscriptionAsync(id);
                    var onSubs = await _subscriptionService.GetOnMeSubscriptionAsync(id);
                    bool amISub = onSubs.Any(sub => sub.FollowerId == me.Id);
                    var profile = _mapper.Map<ProfileModel>(user);
                    profile.Subscribe = amISub;
                    profile.OnMeLikes = onSubs.Count;
                    profile.MyLikes = subs.Count;
                    profile.Role = role;
                    return View(profile);
                }
                else
                {
                    throw new Exception("This user is not exist");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        private async Task AuthenticateAsync(UserDto dto)
        {
            try
            {
                const string authType = "Application Cookie";
                var claims = await _userService.GetUserClamsAsync(dto);
                var identity = new ClaimsIdentity(claims,
                    authType,
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);


                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity));
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }
        }
    }
}

