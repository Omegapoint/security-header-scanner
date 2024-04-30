using System.Net;
using System.Security.Cryptography;
using CloudLookup.Extensions;

namespace CloudLookup.Tests;

public class IPNetworkMergeTests
{
    private static IPNetwork[] Networks(params string[] netStrings) => netStrings.Select(IPNetwork.Parse).ToArray();
    private static object[] TestInput(string[] test, string[] expected) => [Networks(test), Networks(expected)];
    
    public static IEnumerable<object[]> MergeableNetworks => [
        TestInput([], []),
        TestInput(
            [
                "10.0.0.0/24",
                "10.0.1.0/24"
            ],
            ["10.0.0.0/23"]
        ),
        TestInput(
            [
                "10.0.0.0/22",
                "10.0.1.0/24"
            ],
            ["10.0.0.0/22"]
        ),
        TestInput(
            [
                "fe80::/64",
                "fe80:0:0:1::/64"
            ],
            ["fe80::/63"]
        ),
        TestInput(
            [
                "fe80::/62",
                "fe80:0:0:1::/64"
            ],
            ["fe80::/62"]
        ),
        TestInput(
            [
                "10.0.1.0/24",
                "10.0.2.0/24",
                "10.0.3.0/24",
                "10.0.4.0/24"
            ],
            [
                "10.0.1.0/24",
                "10.0.2.0/23",
                "10.0.4.0/24"
            ]
        ),
        TestInput(
            [
                "10.0.0.0/24",
                "10.0.2.0/25",
                "10.0.2.128/25",
                "10.0.4.0/24"
            ],
            [
                "10.0.0.0/24",
                "10.0.2.0/24",
                "10.0.4.0/24"
            ]
        )
    ];

    [Theory]
    [MemberData(nameof(MergeableNetworks))]
    public void HandlesPartiallyMergeableNetworks(IPNetwork[] networks, IPNetwork[] expected)
        => networks.MergeNeighbouring().Should().BeEquivalentTo(expected);

    [Theory]
    [MemberData(nameof(MergeableNetworks))]
    public void InputOrderDoesNotMatter(IPNetwork[] networks, IPNetwork[] expected)
    {
        var seed = RandomNumberGenerator.GetInt32(int.MaxValue);
        var random = new Random(seed);

        for (var i = 0; i < 10; i++)
        {
            random.Shuffle(networks);
            var mergedNetworks = networks.MergeNeighbouring();
            
            mergedNetworks.Should().BeEquivalentTo(expected, $"order should not matter when merging (seed was {seed})");
        }
    }
    
    [Theory]
    [InlineData("10.0.0.0/24", "10.0.2.0/24")]
    [InlineData("fe80::/64", "fe80:0:0:2::/64")]
    [InlineData("10.0.0.0/24", "fe80:0:0:2::/64")]
    public void DoesNotMergeNonMergeableSubnets(string net1, string net2)
    {
        var ipNetwork1 = IPNetwork.Parse(net1);
        var ipNetwork2 = IPNetwork.Parse(net2);

        List<IPNetwork> networks = [ipNetwork1, ipNetwork2];

        networks.MergeNeighbouring().Should().BeEquivalentTo(networks);
    }
}