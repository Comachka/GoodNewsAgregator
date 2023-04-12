using Microsoft.EntityFrameworkCore;
using myProject.Abstractions.Data.Repositories;
using myProject.Core.DTOs;
using myProject.Data;
using myProject.Data.Entities;

namespace myProject.Repositories.Implementations
{
    public class SubscriptionRepository :Repository<Subscription>, ISubscriptionRepository
    {
        public SubscriptionRepository(MyProjectContext myProjectContext)
            : base(myProjectContext)
        {
        }

        public async Task<List<Subscription>> GetSubscriptionsForPageAsync(int page, int pageSize)
        {
            var subscriptions = await _dbSet
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return subscriptions;
        }
       
    }
}