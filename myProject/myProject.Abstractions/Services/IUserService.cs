using myProject.Core.DTOs;

namespace myProject.Abstractions.Services
{
    public interface IUserService
    {
        Task<bool> IsUserExistsAsync(string email);
        Task<UserDto?> RegisterAsync(string email, string password, string aboutMyself, string name, bool mailNotification, string avatar);
        Task<bool> IsPasswordCorrectAsync(string modelEmail, string modelPassword);
        Task<UserDto?> GetUserByEmailAsync(string modelEmail);
        Task<List<UserDto>> GetUsersAsync();
    }
}