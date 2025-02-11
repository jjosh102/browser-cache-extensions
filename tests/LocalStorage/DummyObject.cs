namespace BrowserCache.Extensions.LocalStorage.Tests;
public class DummyObject
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DummyObject()
    {
    }
    public DummyObject(int id, string name)
    {
        Id = id;
        Name = name;
    }
}