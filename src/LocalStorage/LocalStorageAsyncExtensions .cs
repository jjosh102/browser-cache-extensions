using Blazored.LocalStorage;

namespace BrowserCache.Extensions.LocalStorage;

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
        
        try
        {
            var newData = await generateCache();
            var newCache = new LocalCacheItem<T>(newData, timeToLive);
            await localStorageService.SetItemAsync(key, newCache);
            return newData;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Cache generation failed.", ex);
        }
    }

    public static async ValueTask<(bool isCacheExist, T? cacheItem)> TryGetCacheAsync<T>(
        this ILocalStorageService localStorageService,
        string key)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(key);

        var cache = await localStorageService.GetItemAsync<LocalCacheItem<T>>(key);
        return cache is { } existingCache && !existingCache.IsExpired()
            ? (true, existingCache.Data)
            : (false, default);
    }

}
