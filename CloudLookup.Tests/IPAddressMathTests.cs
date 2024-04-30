using System.Net;
using CloudLookup.Extensions;

namespace CloudLookup.Tests;

public class IPAddressMathTests
{
    [Theory]
    [InlineData("10.0.0.1", 1, "10.0.0.2")]
    [InlineData("10.0.0.1", -1, "10.0.0.0")]
    [InlineData("1.2.3.4", 256, "1.2.4.4")]
    [InlineData("1.3.3.7", 0, "1.3.3.7")]
    [InlineData("::1", 1, "::2")]
    [InlineData("f41a:fe1b:055e::420", 3863, "f41a:fe1b:055e::1337")]
    public void AdditionWorks(string ip1, int offset, string ip2)
    {
        var ipAddress1 = IPAddress.Parse(ip1);
        var ipAddress2 = IPAddress.Parse(ip2);

        ipAddress1.Add(offset).Should().Be(ipAddress2);
    }
    
    [Theory]
    [InlineData("10.0.0.1", 1, "10.0.0.0")]
    [InlineData("10.0.0.1", -1, "10.0.0.2")]
    [InlineData("1.2.3.4", 256, "1.2.2.4")]
    [InlineData("1.3.3.7", 0, "1.3.3.7")]
    [InlineData("::2", 1, "::1")]
    [InlineData("f41a:fe1b:055e::1337", 3863, "f41a:fe1b:055e::420")]
    public void SubtractionWorks(string ip1, int offset, string ip2)
    {
        var ipAddress1 = IPAddress.Parse(ip1);
        var ipAddress2 = IPAddress.Parse(ip2);

        ipAddress1.Subtract(offset).Should().Be(ipAddress2);
    }
}