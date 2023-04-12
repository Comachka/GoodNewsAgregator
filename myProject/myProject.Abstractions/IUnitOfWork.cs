using myProject.Abstractions.Data.Repositories;
using myProject.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myProject.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        public IArticleRepository Articles { get; }
        public IRepository<Category> Categories { get; }
        public ICommentRepository Comments { get; }
        public IRepository<NewsResource> NewsResources { get; }
        public IRepository<Role> Roles { get; }
        public ISubscriptionRepository Subscriptions { get; }
        public IRepository<UserCategory> UserCategories { get; }
        public IRepository<User> Users { get; }

        public Task<int> SaveChangesAsync();
    }
}
