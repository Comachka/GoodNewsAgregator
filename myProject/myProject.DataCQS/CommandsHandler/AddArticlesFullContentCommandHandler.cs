using myProject.DataCQS.Commands;
using myProject.Data.Entities;
using AutoMapper;
using MediatR;
using myProject.Data;
using Microsoft.EntityFrameworkCore;
using myProject.Core.DTOs;

namespace myProject.DataCQS.CommandsHandler
{
    public class AddArticlesFullContentCommandHandler : IRequestHandler<AddArticlesFullContentCommand>
    {
        private readonly MyProjectContext _context;
        private readonly IMapper _mapper;

        public AddArticlesFullContentCommandHandler(MyProjectContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task Handle(AddArticlesFullContentCommand command,
            CancellationToken cancellationToken)
        {

            var articlesForUpdate = _context.Articles.Where(article => command.Articles
                .Select(dto => dto.Id)
                .Contains(article.Id))
                .ToList();

            var articlesDict = command.Articles.ToDictionary(dto => dto.Id, dto => dto.Content);

            foreach (var article in articlesForUpdate)
            {
                article.Content = articlesDict[article.Id];

            }
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}