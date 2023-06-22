using MediatR;
using myProject.Core.DTOs;

namespace myProject.DataCQS.Queries
{
    public class GetAllArticlesWithoutContentQuery : IRequest<List<ArticleDto>>
    {

    }
}  