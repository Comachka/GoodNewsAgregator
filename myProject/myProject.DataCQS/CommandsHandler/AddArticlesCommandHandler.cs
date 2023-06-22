using myProject.DataCQS.Commands;
using myProject.Data.Entities;
using AutoMapper;
using MediatR;
using myProject.Data;

namespace myProject.DataCQS.CommandsHandler
{
    public class AddArticlesCommandHandler : IRequestHandler<AddArticlesCommand>
    {
        private readonly MyProjectContext _context;
        private readonly IMapper _mapper;

        public AddArticlesCommandHandler(MyProjectContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task Handle(AddArticlesCommand request,
            CancellationToken cancellationToken)
        {
            var articles = request.Articles.Select(dto => _mapper.Map<Article>(dto)).ToList();
            await _context.Articles.AddRangeAsync(articles, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}