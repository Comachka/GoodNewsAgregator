using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using myProject.Abstractions;
using myProject.Abstractions.Services;
using myProject.Core.DTOs;
using myProject.DataCQS.Commands;
using myProject.DataCQS.Queries;
using myProject.Data.Entities;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using MediatR;
using Serilog;
using Microsoft.Extensions.Logging;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace myProject.Business
{
    public class TokenService : ITokenService
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IMediator mediator,
            IConfiguration configuration,
            ILogger<TokenService> logger)
        {
            _mediator = mediator;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task AddRefreshTokenAsync(int userId, Guid refreshToken)
        {
            try
            {
                await _mediator.Send(new AddRefreshTokenCommand()
                {
                    UserId = userId,
                    Value = refreshToken
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                throw;
            }

        }

        public Task RevokeRefreshTokenAsync(Guid refreshToken)
        {
            throw new NotImplementedException();
        }

        public async Task<TokenDto> RefreshTokenAsync(Guid refreshToken)
        {
            var user = await _mediator.Send(new GetUserByRefreshTokenQuery()
            {
                RefreshToken = refreshToken
            });

            if (user != null)
            {
                var jwt = await GenerateJwtToken(user);
                await _mediator.Send(new RemoveRefreshTokenCommand() { RefreshToken = refreshToken });

                var newRT = Guid.NewGuid();
                await _mediator.Send(new AddRefreshTokenCommand() { UserId = user.Id, Value = newRT });

                return new TokenDto()
                {
                    JwtToken = jwt,
                    Email = user.Email,
                    RefreshToken = newRT.ToString("D")
                };
            }
            throw new ArgumentException("RT not connected with User", nameof(refreshToken));

        }


        private async Task<string> GenerateJwtToken(UserDto user)
        {
            var role = await _mediator.Send(new GetUserRoleNameByUserIdQuery() { UserId = user.Id });
            var claims = new List<Claim>
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
           new Claim(ClaimsIdentity.DefaultRoleClaimType, role),
           new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
           new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("D")),
           new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString("R"))
        };
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:SecurityKey"]));

            var signIn = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["Jwt:ExpireInMinutes"])),
                signingCredentials: signIn);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
