using headers.security.Scanner.Hsts;
using headers.security.Scanner.Hsts.Contracts;

namespace headers.security.Tests;

[Trait("Category", "Integration")]
public class HstsPreloadActualDataTests(HstsPreloadActualDataFixture fixture)
    : IClassFixture<HstsPreloadActualDataFixture>
{
    private readonly List<PreloadEntry> _preloadEntries = fixture.PreloadEntries;

    [Fact]
    public void CanParseActualHstsPreloadList()
    {
        var start = DateTime.UtcNow;
        
        var exception = Record.Exception(() => 
            PreloadPolicyNode.Create(_preloadEntries)
                .Should().NotBeNull()
        );
        
        Assert.Null(exception);

        var stop = DateTime.UtcNow;

        stop.Subtract(start)
            .Should().BeLessThan(TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void CanGetAllEntries()
    {
        var tree = PreloadPolicyNode.Create(_preloadEntries);
        
        var start = DateTime.UtcNow;

        foreach (var entry in _preloadEntries)
        {
            var matchedPolicy = tree
                .GetOrDefault(entry.Domain)
                .Policy;
            
            matchedPolicy.Should().Be(entry);
        }

        var stop = DateTime.UtcNow;

        stop.Subtract(start)
            .Should().BeLessThan(TimeSpan.FromSeconds(2));
    }
}