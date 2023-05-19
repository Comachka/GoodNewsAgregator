using AutoMapper;
using Microsoft.EntityFrameworkCore;
using myProject.Abstractions;
using myProject.Abstractions.Services;
using myProject.Core.DTOs;
using myProject.Data.Entities;

namespace myProject.Business
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> IsRoleExistsAsync(string name)
        {
            return await _unitOfWork.Roles
                .GetAsQueryable()
                .AnyAsync(role => role.Name.Equals(name));
        }

        public async Task<int> GetRoleIdByName(string name)
        {

            var roleId = await _unitOfWork.Roles.FindBy(role => role.Name.Equals(name))
                .Select(role => role.Id)
                .FirstOrDefaultAsync();

            return roleId;
        }

        public async Task InitiateDefaultRolesAsync()
        {
            var isAnyRoleNeedToBeInserted = false;
            if (!await IsRoleExistsAsync("User"))
            {
                isAnyRoleNeedToBeInserted = true;
                await _unitOfWork.Roles.AddAsync(new Role() { Name = "User" });
            }
            if (!await IsRoleExistsAsync("Admin"))
            {
                isAnyRoleNeedToBeInserted = true;
                await _unitOfWork.Roles.AddAsync(new Role() { Name = "Admin" });
            }

            if (isAnyRoleNeedToBeInserted)
            {
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<string?> GetUserRole(int userId)
        {
            var roleId = (await _unitOfWork.Users.GetByIdAsync(userId))?.RoleId;
            if (roleId.HasValue)
            {
                var role = await _unitOfWork.Roles.GetByIdAsync(roleId.Value);
                if (role != null)
                {
                    return role.Name;
                }
            }

            return null;
        }
    }
}