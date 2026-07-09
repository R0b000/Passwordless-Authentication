using Microsoft.Extensions.Caching.Memory;
using PasswordlessApi.Api.Service.Interface.Security;

namespace PasswordlessApi.Api.Service.Implementation.Security
{
    public class InMemoryRateLimiter : IRateLimiter
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<InMemoryRateLimiter> _logger;

        public InMemoryRateLimiter(IMemoryCache cache, ILogger<InMemoryRateLimiter> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task<bool> IsLimitedAsync(string key, int maxRequests, TimeSpan window, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"rate_limit:{key}";
            
            List<DateTime> requests = new();
            if (_cache.TryGetValue(cacheKey, out var cached) && cached is List<DateTime> cachedList)
            {
                requests = cachedList;
            }

            var now = DateTime.UtcNow;
            requests = requests.FindAll(r => now - r < window);

            if (requests.Count >= maxRequests)
            {
                _logger.LogWarning("Rate limit exceeded for key {Key}: {Count} requests in {Window} seconds", 
                    key, requests.Count, window.TotalSeconds);
                return Task.FromResult(true);
            }

            requests.Add(now);
            _cache.Set(cacheKey, requests, window);
            
            return Task.FromResult(false);
        }
    }
}