using myProject.Core.DTOs;

namespace myProject.Abstractions.Services
{
    public interface ISubscriptionService
    {
        Task<List<SubscriptionDto>> GetMySubscriptionAsync(int myId);
        Task<List<SubscriptionDto>> GetOnMeSubscriptionAsync(int myId);
        Task DeleteSubscriptionByIdAsync(int myId, int subId);
        Task AddSubscriptionByIdAsync(int myId, int subId);
        Task<List<SubscriptionDto>> GetSubscriptionsAsync();
    }
}