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
        private readonly IMapper _mapper;

        public RoleService(IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        private async Task<bool> IsRoleExistsAsync(string name)
        {
            return await _unitOfWork.Roles
                .GetAsQueryable()
                .AnyAsync(role => role.Name.Equals(name));
        }

        public async Task<List<RoleDto>> GetRolesAsync()
        {
            return await _unitOfWork.Roles.GetAsQueryable().
                Select(role => _mapper.Map<RoleDto>(role)).ToListAsync();
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
            if (!await IsRoleExistsAsync("Пользователь"))
            {
                isAnyRoleNeedToBeInserted = true;
                await _unitOfWork.Roles.AddAsync(new Role() { Name = "Пользователь" });
            }
            if (!await IsRoleExistsAsync("Администратор"))
            {
                isAnyRoleNeedToBeInserted = true;
                await _unitOfWork.Roles.AddAsync(new Role() { Name = "Администратор" });
            }
            if (!await IsRoleExistsAsync("Модератор"))
            {
                isAnyRoleNeedToBeInserted = true;
                await _unitOfWork.Roles.AddAsync(new Role() { Name = "Модератор" });
            }
            if (!await IsRoleExistsAsync("Главный модератор"))
            {
                isAnyRoleNeedToBeInserted = true;
                await _unitOfWork.Roles.AddAsync(new Role() { Name = "Главный модератор" });
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