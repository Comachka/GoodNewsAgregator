using Microsoft.AspNetCore.Mvc;
using myProject.Models;
using myProject.Abstractions.Services;
using System.Security.Claims;
using myProject.Core.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Web;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

namespace myProject.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public AccountController(IUserService userService,
            IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
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
            var avatar = new byte[] { };
            using (var memoryStream = new MemoryStream())
            {
                await model.Avatar.CopyToAsync(memoryStream);

                // Upload the file if less than 100 KB
                if (memoryStream.Length < 102400)
                {
                    avatar = memoryStream.ToArray();
                }
                else
                {
                    ModelState.AddModelError("File", "The file is too large.");
                }

            }

            if (ModelState.IsValid)
            {
                
                string strAvatar = Convert.ToBase64String(avatar);

                var user = await _userService.RegisterAsync(model.Email, model.Password, model.Name, model.AboutMyself, model.MailNotification, strAvatar);
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
           // var avatar = new MemoryStream(Convert.FromBase64String(user.Avatar));

            if (user != null)
            {
                var model = new MyAccountModel
                {
                    Name = user.Name,
                    Avatar =user.Avatar, //new FileStreamResult(avatar, "image/jpeg"),
                    AboutMyself = user.AboutMyself
                };
                return View(model);
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

