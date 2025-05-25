using System.Net;
using NetTools;

namespace CloudLookup.Tests;

[Trait("Category", "Integration")]
public class CloudLookupActualDataTests(CloudLookupActualDataFixture fixture)
    : IClassFixture<CloudLookupActualDataFixture>
{
    private readonly (Cloud, IEnumerable<IPNetwork>)[] _networks = fixture.Networks;

    [Fact]
    public void CanConstructLookupUsingActualData()
    {
        var lookupCreationAction = () => new CloudLookupCollection(_networks);

        var start = DateTime.UtcNow;
        
        lookupCreationAction
            .Should().NotThrow()
            .And.Subject.Should().NotBeNull();

        if (DateTime.UtcNow.Subtract(start).TotalSeconds >= 5)
        {
            TestContext.Current.AddWarning("Construction of CloudLookupCollection is slow");
        }
    }

    [Fact]
    public void AllVendorsHaveRanges()
    {
        var start = DateTime.UtcNow;

        foreach (var (cloud, networks) in _networks)
        {
            networks.Should().NotBeEmpty($"{cloud} - no networks returned");
        }

        if (DateTime.UtcNow.Subtract(start).TotalSeconds >= 10)
        {
            TestContext.Current.AddWarning("Network lookup in CloudLookupCollection is slow");
        }
    }

    [Fact]
    public void CanGetAllEntries()
    {
        var lookup = new CloudLookupCollection(_networks);
        
        var start = DateTime.UtcNow;

        foreach (var (cloud, networks) in _networks)
        {
            foreach (var network in networks)
            {
                var range = IPAddressRange.Parse(network.ToString());
                lookup.GetCloud(range.Begin).Should().Be(cloud);
                lookup.GetCloud(range.End).Should().Be(cloud);
            }
        }

        if (DateTime.UtcNow.Subtract(start).TotalSeconds >= 20)
        {
            TestContext.Current.AddWarning("IP lookup in CloudLookupCollection is slow");
        }
    }

    [Fact]
    public void EnsureCloudVendorsDontOverlap()
    {
        var lookup = new CloudLookupCollection(_networks);
        var collisions = new List<((Cloud, IPNetwork), (Cloud, IPNetwork))>();
        
        foreach (var (cloud, networks) in _networks)
        {
            foreach (var network in networks)
            {
                var range = IPAddressRange.Parse(network.ToString());
                
                foreach (var otherCloud in Enum.GetValues<Cloud>())
                {
                    if (otherCloud == cloud || otherCloud == Cloud.Unknown) continue;

                    var otherNetworks = lookup.GetNetworks(otherCloud);

                    foreach (var otherNetwork in otherNetworks)
                    {
                        if (otherNetwork.Contains(range.Begin) || otherNetwork.Contains(range.End))
                        {
                            collisions.Add(((cloud, network), (otherCloud, otherNetwork)));
                        }
                    }
                }
            }
        }

        collisions.Should().BeEmpty();
    }
}