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

        var newData = generateCache();
        var newCache = new LocalCacheItem<T>(newData, timeToLive);
        localStorageService.SetItem(key, newCache);
        return newData;
    }

    public static bool TryGetCache<T>(
        this ISyncLocalStorageService localStorageService,
        string key,
        out T? cacheData)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(key);

        var currentCache = localStorageService.GetItem<LocalCacheItem<T>>(key);

        if (currentCache is null || currentCache.IsExpired())
        {
            cacheData = default;
            return false;
        }

        cacheData = currentCache.Data;
        return true;
    }
}
