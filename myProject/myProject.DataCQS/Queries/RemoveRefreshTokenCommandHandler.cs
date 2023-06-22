using myProject.Core.DTOs;
using MediatR;

namespace myProject.DataCQS.Queries
{
    public class GetUserRoleNameByUserIdQuery : IRequest<string>
    {
        public int UserId { get; set; }
    }
}