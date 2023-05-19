using myProject.Core.DTOs;

namespace myProject.Abstractions.Services
{
    public interface IRoleService
    {
        Task<bool> IsRoleExistsAsync(string name);
        Task<int> GetRoleIdByName(string name);
        Task InitiateDefaultRolesAsync();
        Task<string?> GetUserRole(int userId);
    }
}