using myProject.Core.DTOs;

namespace myProject.Abstractions.Services
{
    public interface IJwtService
    {
        Task<string> GetTokenAsync(UserDto user);

    }
}