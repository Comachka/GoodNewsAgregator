using MediatR;

namespace myProject.DataCQS.Commands
{
    public class RemoveRefreshTokenCommand : IRequest
    {
        public Guid RefreshToken { get; set; }
    }
}