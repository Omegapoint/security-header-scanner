using System.Net;
using CloudLookup.Extensions;

namespace CloudLookup.Tests;

public class IPNetworkComparerTests
{
    [Theory]
    [InlineData("10.30.0.0/22", "10.200.0.0/22")]
    [InlineData("10.10.0.0/22", "10.100.0.0/22")]
    [InlineData("10.10.0.0/22", "10.10.0.0/23")]
    [InlineData("1.2.3.4/32", "1.2.3.40/32")]
    [InlineData("1.2.3.4/32", "4.3.2.1/32")]
    [InlineData("::1/128", "::2/128")]
    [InlineData("::/64", "0:0:0:1::/64")]
    [InlineData("::/64", "0:0:0:0::/66")]
    [InlineData("0:0:0:0::/64", "::/66")]
    public void OrdersCorrectly(string first, string last)
    {
        var firstNetwork = IPNetwork.Parse(first);
        var lastNetwork = IPNetwork.Parse(last);

        var comparer = new IPNetworkComparer();

        comparer.Compare(firstNetwork, lastNetwork).Should().BeLessThan(0);
    }
    
    [Theory]
    [InlineData("10.30.0.0/22", "10.30.0.0/22")]
    [InlineData("::/64", "::/64")]
    [InlineData("::/64", "0:0:0:0::/64")]
    public void HandlesEquivalent(string first, string last)
    {
        var firstNetwork = IPNetwork.Parse(first);
        var lastNetwork = IPNetwork.Parse(last);

        var comparer = new IPNetworkComparer();

        comparer.Compare(firstNetwork, lastNetwork).Should().Be(0);
    }
}