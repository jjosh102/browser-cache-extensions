namespace BrowserCache.Extensions.LocalStorage;
public class LocalCacheItem<T>(T? data, TimeSpan? timeToLive = null)
{
    private DateTime _created = DateTime.UtcNow;

    public T? Data { get; init; } = data;

    public TimeSpan? TimeToLive { get; } = timeToLive;

    public DateTime Created
    {
        get => _created;
        set => _created = value;
    }

    public DateTime? ExpiresAt => TimeToLive.HasValue ? _created.Add(TimeToLive.Value) : null;

    public bool IsExpired() => ExpiresAt is { } exp && DateTime.UtcNow >= exp;
}