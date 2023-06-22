using myProject.DataCQS.Commands;
using myProject.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using myProject.Data;

namespace myProject.DataCQS.Commands
{
    public class RemoveRefreshTokenCommandHandler : IRequestHandler<RemoveRefreshTokenCommand>
    {
        private readonly MyProjectContext _context;

        public RemoveRefreshTokenCommandHandler(MyProjectContext context)
        {
            _context = context;
        }

        public async Task Handle(RemoveRefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            var rt = await _context.RefreshTokens.FirstOrDefaultAsync(token => token.Value.Equals(request.RefreshToken),
                cancellationToken);
            _context.RefreshTokens.Remove(rt);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}