using MediatR;

namespace myProject.DataCQS.Commands
{
    public class AddRefreshTokenCommand : IRequest
    {
        public int UserId { get; set; }
        public Guid Value { get; set; }
    }
}