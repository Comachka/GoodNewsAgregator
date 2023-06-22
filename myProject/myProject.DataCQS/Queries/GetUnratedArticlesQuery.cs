using MediatR;
using myProject.Core.DTOs;

namespace myProject.DataCQS.Queries
{
    public class GetUnratedArticlesQuery : IRequest<List<ArticleDto>>
    {

    }
}  