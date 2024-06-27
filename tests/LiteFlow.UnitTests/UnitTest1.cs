using System.Collections.ObjectModel;

namespace LiteFlow.UnitTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
        var X = new ReadOnlyCollection<string>([]);

    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
}