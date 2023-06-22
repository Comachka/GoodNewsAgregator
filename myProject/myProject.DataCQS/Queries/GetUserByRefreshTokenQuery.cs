using MediatR;
using myProject.Core.DTOs;

namespace myProject.DataCQS.Queries
{
    public class GetUserByRefreshTokenQuery : IRequest<UserDto>
    {
        public Guid RefreshToken { get; set; }
    }
}