using myProject.Core.DTOs;
using myProject.DataCQS.Queries;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using myProject.Data;

namespace myProject.DataCQS.QueriesHandlers
{
    public class GetUserByRefreshTokenQueryHandler : IRequestHandler<GetUserByRefreshTokenQuery, UserDto>
    {
        private readonly MyProjectContext _context;
        private readonly IMapper _mapper;

        public GetUserByRefreshTokenQueryHandler(MyProjectContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(GetUserByRefreshTokenQuery request, CancellationToken cancellationToken)
        {
            var userDto = _mapper.Map<UserDto>((await _context.RefreshTokens
                .AsNoTracking()
                .Include(token => token.User)
                .FirstOrDefaultAsync(token => token.Value.Equals(request.RefreshToken),
                    cancellationToken: cancellationToken))?.User);

            return userDto;
        }
    }
}