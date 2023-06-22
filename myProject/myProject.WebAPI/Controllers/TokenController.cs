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
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;
        private readonly ITokenService _tokenService;

        public TokenController(IUserService userService,
            IJwtService jwtService,
            ITokenService tokenService)
        {
            _userService = userService;
            _jwtService = jwtService;
            _tokenService = tokenService;
        }

        [HttpPost]
        [Route("login")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                if (await _userService.IsUserExistsAsync(loginRequest.Email)
               && await _userService.IsPasswordCorrectAsync(loginRequest.Email, loginRequest.Password))
                {
                    var user = await _userService.GetUserByEmailAsync(loginRequest.Email);

                    if (user != null)
                    {
                        var jwtTokenString = await _jwtService.GetTokenAsync(user);

                        var refreshToken = Guid.NewGuid();
                        await _tokenService.AddRefreshTokenAsync(user.Id, refreshToken);


                        return Ok(new TokenResponse()
                        {
                            JwtToken = jwtTokenString,
                            RefreshToken = refreshToken.ToString("D"),
                            Email = user.Email
                        });
                    }
                    else
                    {
                        return BadRequest("Invalid user");
                    }
                }
                else
                {
                    return BadRequest("Invalid user or credentials");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, new ErrorResponse() { Message = ex.Message });
            }

        }

        [HttpPost]
        [Route("refresh-token")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            try
            {
                var tokens = await _tokenService.RefreshTokenAsync(refreshTokenRequest.RefreshToken);

                return Ok(new TokenResponse()
                {
                    JwtToken = tokens.JwtToken,
                    Email = tokens.Email,
                    RefreshToken = tokens.RefreshToken
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                return StatusCode(500, new ErrorResponse() { Message = ex.Message });
            }
            
        }

    }
}
