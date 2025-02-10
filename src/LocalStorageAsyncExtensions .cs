using Blazored.LocalStorage;

namespace LocalStorage.Extensions;

public static class LocalStorageAsyncExtensions
{
    public static async ValueTask<T?> GetOrCreateCacheAsync<T>(
        this ILocalStorageService localStorageService,
        string key,
        TimeSpan timeToLive,
        Func<ValueTask<T>> generateCache)
    {
        var (isCacheExist, cache) = await localStorageService.TryGetCacheAsync<T>(key);
        if (isCacheExist) return cache;

        var newData = await generateCache();
        var newCache = new LocalCacheItem<T>(newData, timeToLive);
        await localStorageService.SetItemAsync(key, newCache);
        return newData;
    }

    public static async ValueTask<(bool isCacheExist, T? cacheItem)> TryGetCacheAsync<T>(
        this ILocalStorageService localStorageService,
        string key)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(key);

        if (!await localStorageService.ContainKeyAsync(key))
            return (false, default);

        var cache = await localStorageService.GetItemAsync<LocalCacheItem<T>>(key);
        return cache is not null && !cache.IsExpired()
            ? (true, cache.Data)
            : (false, default);
    }
}
