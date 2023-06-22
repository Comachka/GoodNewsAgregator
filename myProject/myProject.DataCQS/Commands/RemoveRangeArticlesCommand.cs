using myProject.Core.DTOs;
using MediatR;

namespace myProject.DataCQS.Commands
{
    public class RemoveRangeArticlesCommand : IRequest
    {
        public IEnumerable<int> ArticlesId { get; set; }
    }
}