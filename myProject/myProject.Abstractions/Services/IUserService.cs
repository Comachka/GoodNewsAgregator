using myProject.Core.DTOs;

namespace myProject.Abstractions.Services
{
    public interface IUserService
    {
        Task<bool> IsUserExistsAsync(string email);
        Task<UserDto?> RegisterAsync(string email, string password, string aboutMyself, string name, bool mailNotification, string avatar);
        Task<bool> IsPasswordCorrectAsync(string modelEmail, string modelPassword);
        Task<UserDto?> GetUserByEmailAsync(string modelEmail);
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<List<UserDto>> GetUsersAsync();
        Task ChangeRaiting(int id, int raiting);
    }
}