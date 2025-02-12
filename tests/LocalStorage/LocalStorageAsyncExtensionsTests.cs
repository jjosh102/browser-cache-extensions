using Blazored.LocalStorage;
using NSubstitute;

namespace BrowserCache.Extensions.LocalStorage.Tests;

public class LocalStorageAsyncExtensionsTests
{
    private readonly ILocalStorageService _localStorageService;

    public LocalStorageAsyncExtensionsTests()
    {
        _localStorageService = Substitute.For<ILocalStorageService>();
    }

    [Fact]
    public async Task GetOrCreateCacheAsync_ReturnsCachedData_WhenCacheExistsAndIsNotExpired()
    {
        // Arrange
        var key = "testKey";
        var cachedData = new DummyObject(1, "Cached Data");
        var cacheItem = new LocalCacheItem<DummyObject>(cachedData, TimeSpan.FromMinutes(10));
        _localStorageService.GetItemAsync<LocalCacheItem<DummyObject>>(key).Returns(cacheItem);

        // Act
        var result =
            await _localStorageService.GetOrCreateCacheAsync<DummyObject>(key, TimeSpan.FromMinutes(10),
                () => DummyFunction());

        // Assert
        Assert.Equal(cachedData.Id, result?.Id);
        Assert.Equal(cachedData.Name, result?.Name);
        await _localStorageService.DidNotReceive().SetItemAsync(key, Arg.Any<LocalCacheItem<DummyObject>>());
    }

    [Fact]
    public async Task GetOrCreateCacheAsync_CreatesAndReturnsNewData_WhenCacheDoesNotExist()
    {
        // Arrange
        var key = "testKey";
        var newData = new DummyObject(2, "New Data");
        _localStorageService.GetItemAsync<LocalCacheItem<DummyObject>>(key).Returns((LocalCacheItem<DummyObject>?)null);

        // Act
        var result = await _localStorageService.GetOrCreateCacheAsync<DummyObject>(key, TimeSpan.FromMinutes(10),
            () => new ValueTask<DummyObject>(newData));

        // Assert
        Assert.Equal(newData.Id, result?.Id);
        Assert.Equal(newData.Name, result?.Name);
        await _localStorageService.Received(1)
            .SetItemAsync(key, Arg.Is<LocalCacheItem<DummyObject>>(item => item.Data == newData));
    }

    [Fact]
    public async Task GetOrCreateCacheAsync_CreatesAndReturnsNewData_WhenCacheIsExpired()
    {
        // Arrange
        var key = "testKey";
        var expiredData = new DummyObject(3, "Expired Data");
        var expiredCacheItem = new LocalCacheItem<DummyObject>(expiredData, TimeSpan.FromMinutes(-10));
        var newData = new DummyObject(4, "New Data");
        _localStorageService.GetItemAsync<LocalCacheItem<DummyObject>>(key).Returns(expiredCacheItem);

        // Act
        var result = await _localStorageService.GetOrCreateCacheAsync<DummyObject>(key, TimeSpan.FromMinutes(10),
            () => new ValueTask<DummyObject>(newData));

        // Assert
        Assert.Equal(newData.Id, result?.Id);
        Assert.Equal(newData.Name, result?.Name);
        await _localStorageService.Received(1)
            .SetItemAsync(key, Arg.Is<LocalCacheItem<DummyObject>>(item => item.Data == newData));
    }

    [Fact]
    public async Task GetOrCreateCacheAsync_ThrowsInvalidOperationException_WhenDataGenerationFails()
    {
        // Arrange
        var key = "testKey";
        _localStorageService.GetItemAsync<LocalCacheItem<DummyObject>>(key)
            .Returns((LocalCacheItem<DummyObject>?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await _localStorageService.GetOrCreateCacheAsync<DummyObject>(
                key,
                TimeSpan.FromMinutes(10),
                () => throw new Exception("Generation failed"));
        });
    }


    private ValueTask<DummyObject> DummyFunction()
    {
        return new ValueTask<DummyObject>(new DummyObject(5, "Dummy"));
    }
}