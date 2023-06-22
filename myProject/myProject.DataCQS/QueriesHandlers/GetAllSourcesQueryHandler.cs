using myProject.Core.DTOs;
using myProject.DataCQS.Queries;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using myProject.Data;

namespace myProject.DataCQS.QueriesHandlers
{
    public class GetAllSourcesQueryHandler : IRequestHandler<GetAllSourcesQuery, List<NewsResourceDto>>
    {
        private readonly MyProjectContext _context;
        private readonly IMapper _mapper;

        public GetAllSourcesQueryHandler(MyProjectContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<NewsResourceDto>> Handle(GetAllSourcesQuery request, CancellationToken cancellationToken)
        {
            var sourceDtos = await _context.NewsResources
                .AsNoTracking()
                .Select(source => _mapper.Map<NewsResourceDto>(source))
                .ToListAsync(cancellationToken: cancellationToken);

            return sourceDtos;
        }
    }
}