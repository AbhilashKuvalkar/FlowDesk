using FlowDesk.TicketService.Domain.Common;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FlowDesk.TicketService.Infrastructure.Caching;

public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;

    private readonly JsonSerializerSettings _jsonSerializerSettings;

    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromMinutes(30);

    public RedisCacheService(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));

        _jsonSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() }
        };
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var cached = await _distributedCache.GetStringAsync(key, cancellationToken);

        return cached is null 
                ? default
                : JsonConvert.DeserializeObject<T>(cached, _jsonSerializerSettings);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _distributedCache.RemoveAsync(key, cancellationToken);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        var serialized = JsonConvert.SerializeObject(value, _jsonSerializerSettings);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry ?? DefaultExpiry
        };

        await _distributedCache.SetStringAsync(key, serialized, options, cancellationToken);
    }
}
