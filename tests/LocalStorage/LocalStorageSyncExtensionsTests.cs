using Blazored.LocalStorage;
using NSubstitute;

namespace BrowserCache.Extensions.LocalStorage.Tests;

public class LocalStorageSyncExtensionsTests
{
    private readonly ISyncLocalStorageService _localStorageService;

    public LocalStorageSyncExtensionsTests()
    {
        _localStorageService = Substitute.For<ISyncLocalStorageService>();
    }

    [Fact]
    public void GetOrCreateCache_ReturnsCachedData_WhenCacheExistsAndIsNotExpired()
    {
        // Arrange
        var key = "testKey";
        var cachedData = new DummyObject(1, "Cached Data");
        var cacheItem = new LocalCacheItem<DummyObject>(cachedData, TimeSpan.FromMinutes(10));
        _localStorageService.GetItem<LocalCacheItem<DummyObject>>(key).Returns(cacheItem);

        // Act
        var result = _localStorageService.GetOrCreateCache<DummyObject>(key, TimeSpan.FromMinutes(10), () => DummyFunction());

        // Assert
        Assert.Equal(cachedData.Id, result?.Id);
        Assert.Equal(cachedData.Name, result?.Name);
        _localStorageService.DidNotReceive().SetItem(key, Arg.Any<LocalCacheItem<DummyObject>>());
    }

    [Fact]
    public void GetOrCreateCache_CreatesAndReturnsNewData_WhenCacheDoesNotExist()
    {
        // Arrange
        var key = "testKey";
        var newData = new DummyObject(2, "New Data");
        _localStorageService.GetItem<LocalCacheItem<DummyObject>>(key).Returns((LocalCacheItem<DummyObject>?)null);

        // Act
        var result = _localStorageService.GetOrCreateCache<DummyObject>(key, TimeSpan.FromMinutes(10), () => newData);

        // Assert
        Assert.Equal(newData.Id, result?.Id);
        Assert.Equal(newData.Name, result?.Name);
        _localStorageService.Received(1).SetItem(key, Arg.Is<LocalCacheItem<DummyObject>>(item => item.Data == newData));
    }

    [Fact]
    public void GetOrCreateCache_CreatesAndReturnsNewData_WhenCacheIsExpired()
    {
        // Arrange
        var key = "testKey";
        var expiredData = new DummyObject(3, "Expired Data");
        var expiredCacheItem = new LocalCacheItem<DummyObject>(expiredData, TimeSpan.FromMinutes(-10));
        var newData = new DummyObject(4, "New Data");
        _localStorageService.GetItem<LocalCacheItem<DummyObject>>(key).Returns(expiredCacheItem);

        // Act
        var result = _localStorageService.GetOrCreateCache<DummyObject>(key, TimeSpan.FromMinutes(10), () => newData);

        // Assert
        Assert.Equal(newData.Id, result?.Id);
        Assert.Equal(newData.Name, result?.Name);
        _localStorageService.Received(1).SetItem(key, Arg.Is<LocalCacheItem<DummyObject>>(item => item.Data == newData));
    }

    [Fact]
    public void GetOrCreateCache_ThrowsInvalidOperationException_WhenDataGenerationFails()
    {
        // Arrange
        var key = "testKey";
        _localStorageService.GetItem<LocalCacheItem<DummyObject>>(key).Returns((LocalCacheItem<DummyObject>?)null);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            _localStorageService.GetOrCreateCache<DummyObject>(key, TimeSpan.FromMinutes(10), () => throw new Exception("Generation failed")));
    }

    private DummyObject DummyFunction()
    {
        return new DummyObject(5, "Dummy");
    }
}


