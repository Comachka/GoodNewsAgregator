using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using myProject.Abstractions;
using myProject.Abstractions.Services;
using myProject.Core.DTOs;
using myProject.Data.Entities;
using System.Text;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace myProject.Business
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork,
            IMapper mapper,
            IRoleService roleService, 
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _roleService = roleService;
            _configuration = configuration;
        }

        public async Task<bool> IsUserExistsAsync(string email)
        {
            var user = await _unitOfWork.Users.FindBy(user => user.Email.Equals(email)).FirstOrDefaultAsync();

            return user != null;
        }

        public async Task ChangeRaiting(int id, int raiting)
        {
            var user = await _unitOfWork.Users.FindBy(user => user.Id.Equals(id)).FirstOrDefaultAsync();
            await _unitOfWork.Users.PatchAsync(id, new List<PatchDto>()
            {
                new PatchDto()
                {
                    PropertyName = nameof(UserDto.Raiting),
                    PropertyValue = user.Raiting + raiting
                }
            });
            if (user.RoleId != 1)
            {
                if (((user.Raiting + raiting) >= 10000) && ((user.Raiting + raiting) < 100000))
                {
                    await _unitOfWork.Users.PatchAsync(id, new List<PatchDto>()
                    {
                        new PatchDto()
                        {
                        PropertyName = nameof(UserDto.RoleId),
                        PropertyValue = 2
                        }
                    });
                }
                else if (((user.Raiting + raiting) >= 100000))
                {
                    await _unitOfWork.Users.PatchAsync(id, new List<PatchDto>()
                    {
                        new PatchDto()
                        {
                        PropertyName = nameof(UserDto.RoleId),
                        PropertyValue = 4
                        }
                    });
                }
                else if (((user.Raiting + raiting) < 10000))
                {
                    await _unitOfWork.Users.PatchAsync(id, new List<PatchDto>()
                    {
                        new PatchDto()
                        {
                        PropertyName = nameof(UserDto.RoleId),
                        PropertyValue = 3
                        }
                    });
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> IsPasswordCorrectAsync(string email, string password)
        {
            if (await IsUserExistsAsync(email))
            {
                var passwordHash = GetPasswordHash(password);
                var user = await _unitOfWork.Users
                    .GetAsQueryable()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(user
                        => user.Email.Equals(email)
                           && user.Password.Equals(passwordHash));

                if (user != null)
                {
                    return true;
                }

                return false;
            }
            else
            {
                throw new ArgumentException("User with that email does not exist", nameof(email));
            }
        }

        public async Task<UserDto?> RegisterAsync(string email, string password, string aboutMyself, string name, bool mailNotification, string avatar)
        {
            if (!await IsUserExistsAsync(email))
            {
                var userRoleId = await _roleService.GetRoleIdByName("User");
                if (userRoleId == 0)
                {
                    //throw new DataException("Role User does not exist");
                    await _roleService.InitiateDefaultRolesAsync();
                }

                var user = new User()
                {
                    Email = email,
                    Password = GetPasswordHash(password),
                    Name = name,
                    AboutMyself = aboutMyself,
                    MailNotification = mailNotification,
                    Raiting = 0,
                    Avatar = avatar,
                    RoleId = userRoleId
                };

                var userEntry = await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return _mapper.Map<UserDto>(await _unitOfWork.Users.GetByIdAsync(userEntry.Entity.Id));
            }

            return null;
        }

        public async Task<UserDto?> GetUserByEmailAsync(string modelEmail)
        {
            var user = await _unitOfWork.Users
                .FindBy(user => user.Email.Equals(modelEmail))
                .FirstOrDefaultAsync();
            return
                user != null
                    ? _mapper.Map<UserDto>(user)
                    : null;
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.Users
                .FindBy(user => user.Id.Equals(id))
                .FirstOrDefaultAsync();
            return
                user != null
                    ? _mapper.Map<UserDto>(user)
                    : null;
        }

        public async Task<List<UserDto>> GetUsersAsync()
        {
            return await _unitOfWork.Users.GetAsQueryable().
                Select(user => _mapper.Map<UserDto>(user)).ToListAsync();
        }

        public async Task ChangeProfileAsync(string modelAvatar, string aboutMyself, string name, bool mailNotification, int id)
        {
            var user = await GetUserByIdAsync(id);
            if (!user.AboutMyself.Equals(aboutMyself))
            {
                await _unitOfWork.Users.PatchAsync(id, new List<PatchDto>()
                {
                    new PatchDto()
                    {
                    PropertyName = nameof(UserDto.AboutMyself),
                    PropertyValue = aboutMyself
                    }
                });
            }
            if (!user.Name.Equals(name))
            {
                await _unitOfWork.Users.PatchAsync(id, new List<PatchDto>()
                {
                    new PatchDto()
                    {
                    PropertyName = nameof(UserDto.Name),
                    PropertyValue = name
                    }
                });
            }
            if (!user.MailNotification.Equals(mailNotification))
            {
                await _unitOfWork.Users.PatchAsync(id, new List<PatchDto>()
                {
                    new PatchDto()
                    {
                    PropertyName = nameof(UserDto.MailNotification),
                    PropertyValue = mailNotification
                    }
                });
            }
            if (modelAvatar != "")
            {
                await _unitOfWork.Users.PatchAsync(id, new List<PatchDto>()
                {
                    new PatchDto()
                    {
                    PropertyName = nameof(UserDto.Avatar),
                    PropertyValue = modelAvatar
                    }
                });

            }
            
            await _unitOfWork.SaveChangesAsync();
        }


        private string GetPasswordHash(string password)
        {
            var sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                var encoding = Encoding.UTF8;
                var result = hash
                    .ComputeHash(
                        encoding
                            .GetBytes($"{password}{_configuration["Secrets:Salt"]}"));

                foreach (var b in result)
                {
                    sb.Append(b.ToString("x2"));
                }
            }
            return sb.ToString();
        }

        public async Task DeleteUserByIdAsync(int id)
        {
            var subs = await _unitOfWork.Subscriptions.FindBy(s => (s.FollowerId == id) || (s.FollowOnId == id)).ToListAsync();
            if (!subs.IsNullOrEmpty())
            {
                await _unitOfWork.Subscriptions.RemoveRange(subs);
            }
            await _unitOfWork.Users.Remove(id);
            await _unitOfWork.SaveChangesAsync();
            return;
        }

    }
}
