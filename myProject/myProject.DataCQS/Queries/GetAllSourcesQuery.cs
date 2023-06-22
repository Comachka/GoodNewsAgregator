using MediatR;
using myProject.Core.DTOs;

namespace myProject.DataCQS.Queries
{
    public class GetAllSourcesQuery : IRequest<List<NewsResourceDto>>
    {

    }
}  