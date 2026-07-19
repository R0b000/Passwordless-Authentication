namespace API.Shared.Service.Interface.Security
{
    public interface IRateLimiter
    {
        Task<bool> IsLimitedAsync(string key, int maxRequests, TimeSpan window, CancellationToken cancellationToken = default);
    }
}