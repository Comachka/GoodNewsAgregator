using myProject.Core.DTOs;
using MediatR;

namespace myProject.DataCQS.Commands
{
    public class RateArticlesCommand : IRequest
    {
        public IEnumerable<ArticleDto> Articles { get; set; }
    }
}