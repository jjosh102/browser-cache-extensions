namespace LocalStorage.Extensions;
public record LocalCacheItem<T>(T? Data, TimeSpan? TimeToLive = null)
{
    public DateTime Created { get; } = DateTime.UtcNow;

    public DateTime? ExpiresAt => TimeToLive.HasValue ? Created.Add(TimeToLive.Value) : null;

    public bool IsExpired() => ExpiresAt is not { } exp || DateTime.UtcNow >= exp;

}
