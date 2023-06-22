using myProject.DataCQS.Commands;
using myProject.Data.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using myProject.Data;

namespace myProject.DataCQS.Commands
{
    public class RemoveRangeArticlesCommandHandler : IRequestHandler<RemoveRangeArticlesCommand>
    {
        private readonly MyProjectContext _context;

        public RemoveRangeArticlesCommandHandler(MyProjectContext context)
        {
            _context = context;
        }

        public async Task Handle(RemoveRangeArticlesCommand request,
            CancellationToken cancellationToken)
        {
            var articles = await _context.Articles.Where(a => request.ArticlesId.Contains(a.Id)).ToListAsync(cancellationToken);

            _context.Articles.RemoveRange(articles);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}