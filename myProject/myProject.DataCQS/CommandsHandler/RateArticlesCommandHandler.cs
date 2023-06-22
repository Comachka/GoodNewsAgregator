using myProject.DataCQS.Commands;
using myProject.Data.Entities;
using AutoMapper;
using MediatR;
using myProject.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace myProject.DataCQS.CommandsHandler
{
    public class RateArticlesCommandHandler : IRequestHandler<RateArticlesCommand>
    {
        private readonly MyProjectContext _context;
        private readonly IMapper _mapper;

        public RateArticlesCommandHandler(MyProjectContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task Handle(RateArticlesCommand command,
            CancellationToken cancellationToken)
        {
            var articlesForUpdate = _context.Articles.Where(article => command.Articles
                .Select(dto => dto.Id)
            .Contains(article.Id))
            .ToList();

            var articlesDict = command.Articles.ToDictionary(dto => dto.Id, dto => dto.PositiveRaiting);

            foreach (var article in articlesForUpdate)
            {
                article.PositiveRaiting = articlesDict[article.Id];

            }
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}