using myProject.Core.DTOs;
using myProject.DataCQS.Queries;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using myProject.Data;

namespace myProject.DataCQS.QueriesHandlers
{
    public class GetAllArticlesWithoutContentQueryHandler : IRequestHandler<GetAllArticlesWithoutContentQuery, List<ArticleDto>>
    {
        private readonly MyProjectContext _context;
        private readonly IMapper _mapper;

        public GetAllArticlesWithoutContentQueryHandler(MyProjectContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ArticleDto>> Handle(GetAllArticlesWithoutContentQuery request, CancellationToken cancellationToken)
        {
            var articleDtos = await _context.Articles
                .AsNoTracking()
                .Where(article => string.IsNullOrEmpty(article.Content))
                .Select(source => _mapper.Map<ArticleDto>(source))
                .ToListAsync(cancellationToken: cancellationToken);

            return articleDtos;
        }
    }
}