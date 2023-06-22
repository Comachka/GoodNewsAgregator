using myProject.Core.DTOs;

namespace myProject.Abstractions.Services
{
    public interface ITokenService
    {
        Task AddRefreshTokenAsync(int userId, Guid refreshToken);
        Task RevokeRefreshTokenAsync(Guid refreshToken);
        Task<TokenDto> RefreshTokenAsync(Guid refreshToken);
    }
}