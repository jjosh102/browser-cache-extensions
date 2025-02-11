# BrowserCacheExtensions
BrowserCacheExtensions is a collection of extensions designed to cache non-confidential data in the browser using popular libraries like [Blazored LocalStorage](https://github.com/Blazored/LocalStorage). Future plans include extending support to [Blazor.IndexedDB](https://github.com/wtulloch/Blazor.IndexedDB).

## Usage

### LocalStorageAsyncExtensions

The `LocalStorageAsyncExtensions` class provides asynchronous methods for caching data. Here's how to use it:

```csharp
using Blazored.LocalStorage;
using LocalStorage.Extensions;
using System;
using System.Threading.Tasks;

public class Example
{
    private readonly ILocalStorageService _localStorageService;

    public Example(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public async Task UseCacheAsync()
    {
        string key = "cachedData";
        TimeSpan timeToLive = TimeSpan.FromMinutes(10);

        var data = await _localStorageService.GetOrCreateCacheAsync(
            key,
            timeToLive,
            async () => await FetchDataAsync()
        );

        // Use 'data' as needed
    }

    private async Task<MyDataType> FetchDataAsync()
    {
        // Fetch or generate data
        return await Task.FromResult(new MyDataType());
    }
}
```

### LocalStorageSyncExtensions

The `LocalStorageSyncExtensions` class offers synchronous methods for caching data:

```csharp
using Blazored.LocalStorage;
using LocalStorage.Extensions;
using System;

public class Example
{
    private readonly ISyncLocalStorageService _localStorageService;

    public Example(ISyncLocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public void UseCache()
    {
        string key = "cachedData";
        TimeSpan timeToLive = TimeSpan.FromMinutes(10);

        var data = _localStorageService.GetOrCreateCache(
            key,
            timeToLive,
            () => FetchData()
        );

        // Use 'data' as needed
    }

    private MyDataType FetchData()
    {
        // Fetch or generate data
        return new MyDataType();
    }
}
```

## Note
If you prefer not to rely on this library, you can copy the extension methods directly into your project and modify them as needed to suit your requirements.

## Disclaimer
Use only this library for caching non sensitive data.
If you are working with highly private and confidential data , you should not be storing this data in your client's browser.

# License
MIT License





