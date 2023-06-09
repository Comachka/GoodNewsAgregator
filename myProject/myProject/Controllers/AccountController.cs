using Microsoft.AspNetCore.Mvc;
using myProject.Models;
using myProject.Abstractions.Services;
using System.Security.Claims;
using myProject.Core.DTOs;
using myProject.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Web;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using Microsoft.AspNetCore.Hosting.Server;
using System.IO;
using AutoMapper;

namespace myProject.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public AccountController(IUserService userService,
            IRoleService roleService,
            IMapper mapper)
        {
            _userService = userService;
            _roleService = roleService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
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
                Directory.CreateDirectory($"{Environment.CurrentDirectory}\\wwwroot\\img\\Avatars\\{model.Email}\\");
                var fileName = Path.GetFileName(model.Avatar.FileName);
                var filePath = Path.Combine($"{Environment.CurrentDirectory}\\wwwroot\\img\\Avatars\\{model.Email}\\", fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Avatar.CopyToAsync(fileStream);
                }
                var modelAvatar = Path.Combine($"\\img\\Avatars\\{model.Email}", fileName);
                var user = await _userService.RegisterAsync(model.Email,
                    model.Password,
                    model.Name,
                    model.AboutMyself,
                    model.MailNotification,
                    modelAvatar);
                if (user != null)
                {
                    await AuthenticateAsync(user);
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Smth goes wrong");
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
        public async Task<IActionResult> Login(LoginModel model)
        {
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

            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> CheckIsUserEmailIsValidAndNotExists(string email)
        {
            return Ok(!await _userService.IsUserExistsAsync(email));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageUsers()
        {
            return Ok(await _userService.GetUsersAsync());
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyAccount()
        {
            var user = await _userService.GetUserByEmailAsync(HttpContext.User.Identity.Name);

            if (user != null)
            {
                
                return View(_mapper.Map<MyAccountModel>(user));
            }
            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user.Email == HttpContext.User.Identity.Name) return RedirectToAction("MyAccount", "Account");
            if (user != null)
            {
                return View(_mapper.Map<ProfileModel>(user));
            }
            return View();
        }

        private async Task AuthenticateAsync(UserDto dto)
        {
            try
            {
                const string authType = "Application Cookie";
                var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, dto.Email),
                };
                var role = await _roleService.GetUserRole(dto.Id);
                if (string.IsNullOrEmpty(role))
                {
                    throw new ArgumentException("Incorrect user or role", nameof(dto));
                }
                claims.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, role));

                var identity = new ClaimsIdentity(claims,
                    authType,
                    ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);


                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity));
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}

