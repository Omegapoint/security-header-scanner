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
        var treeCreationAction = () => PreloadPolicyNode.Create(_preloadEntries);

        var start = DateTime.UtcNow;
        
        treeCreationAction
            .Should().NotThrow()
            .And.Subject.Should().NotBeNull();

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
            tree.GetOrDefault(entry.Domain).Should().Be(entry);
        }

        var stop = DateTime.UtcNow;

        stop.Subtract(start)
            .Should().BeLessThan(TimeSpan.FromSeconds(2));
    }
}