namespace BrowserCache.Extensions.LocalStorage;
public class LocalCacheItem<T>(T? data, TimeSpan? timeToLive = null)
{
    public T? Data { get; init; } = data;

    public TimeSpan? TimeToLive { get; init; } = timeToLive;

    public DateTime Created { get; init; } = DateTime.UtcNow;

    public DateTime? ExpiresAt => TimeToLive.HasValue ? Created.Add(TimeToLive.Value) : null;

    public bool IsExpired() => ExpiresAt is { } exp && DateTime.UtcNow >= exp;
}