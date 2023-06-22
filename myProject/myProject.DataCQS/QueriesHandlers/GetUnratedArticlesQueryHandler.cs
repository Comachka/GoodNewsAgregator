using myProject.Core.DTOs;
using myProject.DataCQS.Queries;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using myProject.Data;

namespace myProject.DataCQS.QueriesHandlers
{
    public class GetUnratedArticlesQueryHandler : IRequestHandler<GetUnratedArticlesQuery, List<ArticleDto>>
    {
        private readonly MyProjectContext _context;
        private readonly IMapper _mapper;

        public GetUnratedArticlesQueryHandler(MyProjectContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ArticleDto>> Handle(GetUnratedArticlesQuery request, CancellationToken cancellationToken)
        {
            var unratedArticles = await _context.Articles
                .AsNoTracking()
                .Where(article => article.PositiveRaiting == null)
                .Select(article => _mapper.Map<ArticleDto>(article))
                .ToListAsync(cancellationToken: cancellationToken);

            return unratedArticles;
        }
    }
}