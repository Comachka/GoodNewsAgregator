using myProject.DataCQS.Commands;
using myProject.Data.Entities;
using MediatR;
using myProject.Data;

namespace myProject.DataCQS.Commands
{
    public class AddRefreshTokenCommandHandler : IRequestHandler<AddRefreshTokenCommand>
    {
        private readonly MyProjectContext _context;

        public AddRefreshTokenCommandHandler(MyProjectContext context)
        {
            _context = context;
        }

        public async Task Handle(AddRefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            await _context.RefreshTokens.AddAsync(new RefreshToken()
            {
                UserId = request.UserId,
                Value = request.Value
            }, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}