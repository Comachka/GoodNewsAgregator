using myProject.Core.DTOs;
using myProject.DataCQS.Queries;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using myProject.Data;

namespace myProject.DataCQS.QueriesHandlers
{
    public class GetUserRoleNameByUserIdQueryHandler : IRequestHandler<GetUserRoleNameByUserIdQuery, string>
    {
        private readonly MyProjectContext _context;

        public GetUserRoleNameByUserIdQueryHandler(MyProjectContext context, IMapper mapper)
        {
            _context = context;
        }

        public async Task<string> Handle(GetUserRoleNameByUserIdQuery request, CancellationToken cancellationToken)
        {
            var roleName = (await _context.Users
                .AsNoTracking()
                .Include(user => user.Role)
                .FirstOrDefaultAsync(user => user.Id.Equals(request.UserId),
                    cancellationToken: cancellationToken))?.Role.Name;
            if (roleName != null)
            {
                return roleName;
            }
            else
            {
                throw new ArgumentException("Incorrect User Id", nameof(request.UserId));
            }
        }
    }
}