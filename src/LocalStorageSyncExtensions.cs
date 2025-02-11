using Blazored.LocalStorage;

namespace LocalStorage.Extensions;

public static class LocalStorageSyncExtensions
{
    public static T? GetOrCreateCache<T>(
        this ISyncLocalStorageService localStorageService,
        string key,
        TimeSpan timeToLive,
        Func<T> generateCache)
    {

        if (localStorageService.TryGetCache(key, out T? cache))
            return cache;

        try
        {
            var newData = generateCache();
            var newCache = new LocalCacheItem<T>(newData, timeToLive);
            localStorageService.SetItem(key, newCache);
            return newData;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Cache generation failed.", ex);
        }
    }

    public static bool TryGetCache<T>(
        this ISyncLocalStorageService localStorageService,
        string key,
        out T? cacheData)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(key);

        var currentCache = localStorageService.GetItem<LocalCacheItem<T>>(key);

        if (currentCache is { } existingCache && !existingCache.IsExpired())
        {
            cacheData = existingCache.Data;
            return true;
        }

        cacheData = default;
        return false;
    }
}
