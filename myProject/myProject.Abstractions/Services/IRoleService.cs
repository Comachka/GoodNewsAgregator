using myProject.Core.DTOs;

namespace myProject.Abstractions.Services
{
    public interface IRoleService
    {
        Task<int> GetRoleIdByName(string name);
        Task InitiateDefaultRolesAsync();
        Task<string?> GetUserRole(int userId);
        Task<List<RoleDto>> GetRolesAsync();
    }
}