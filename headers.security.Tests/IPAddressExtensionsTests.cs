using System.Diagnostics.CodeAnalysis;
using System.Net;
using headers.security.Common.Extensions;

namespace headers.security.Tests;

[SuppressMessage("ReSharper", "InvokeAsExtensionMethod")]
// ReSharper disable once InconsistentNaming
public class IPAddressExtensionsTests
{
    [Theory]
    [InlineData("1.1.1.1")]
    [InlineData("8.8.8.8")]
    [InlineData("100.128.0.1")]
    [InlineData("172.32.0.1")]
    public void IsPrivate_ReturnsFalse_PublicIPv4(string ip)
    {
        var ipAddress = IPAddress.Parse(ip);
        IPAddressExtensions.IsPrivate(ipAddress).Should().BeFalse();
    }

    [Theory]
    [InlineData("::ffff:1.1.1.1")]
    [InlineData("::ffff:8.8.8.8")]
    [InlineData("::ffff:100.128.0.1")]
    [InlineData("::ffff:172.32.0.1")]
    public void IsPrivate_ReturnsFalse_PublicIPv4MappedToIPv6(string ip)
    {
        var ipAddress = IPAddress.Parse(ip);
        IPAddressExtensions.IsPrivate(ipAddress).Should().BeFalse();
    }

    [Theory]
    [InlineData("2606:4700:4700::64")]
    [InlineData("2001:4860:4860::8888")]
    public void IsPrivate_ReturnsFalse_PublicIPv6(string ip)
    {
        var ipAddress = IPAddress.Parse(ip);
        IPAddressExtensions.IsPrivate(ipAddress).Should().BeFalse();
    }

    [Theory]
    [InlineData("127.0.0.1")]
    [InlineData("127.0.0.2")]
    [InlineData("127.10.20.30")]
    [InlineData("127.255.255.255")]
    [InlineData("10.0.0.0")]
    [InlineData("10.20.30.40")]
    [InlineData("10.255.255.255")]
    [InlineData("100.64.0.0")]
    [InlineData("100.64.30.40")]
    [InlineData("100.64.127.255")]
    [InlineData("172.16.0.0")]
    [InlineData("172.20.30.40")]
    [InlineData("172.31.255.255")]
    [InlineData("192.168.0.0")]
    [InlineData("192.168.30.40")]
    [InlineData("192.168.255.255")]
    [InlineData("169.254.0.0")]
    [InlineData("169.254.30.40")]
    [InlineData("169.254.255.255")]
    public void IsPrivate_ReturnsTrue_PrivateIPv4(string ip)
    {
        var ipAddress = IPAddress.Parse(ip);
        IPAddressExtensions.IsPrivate(ipAddress).Should().BeTrue();
    }

    [Theory]
    [InlineData("::ffff:127.0.0.1")]
    [InlineData("::ffff:127.0.0.2")]
    [InlineData("::ffff:127.10.20.30")]
    [InlineData("::ffff:127.255.255.254")]
    [InlineData("::ffff:10.0.0.0")]
    [InlineData("::ffff:10.20.30.40")]
    [InlineData("::ffff:10.255.255.255")]
    [InlineData("::ffff:172.16.0.0")]
    [InlineData("::ffff:172.20.30.40")]
    [InlineData("::ffff:172.31.255.255")]
    [InlineData("::ffff:192.168.0.0")]
    [InlineData("::ffff:192.168.30.40")]
    [InlineData("::ffff:192.168.255.255")]
    [InlineData("::ffff:169.254.0.0")]
    [InlineData("::ffff:169.254.30.40")]
    [InlineData("::ffff:169.254.255.255")]
    public void IsPrivate_ReturnsTrue_PrivateIPv4MappedToIPv6(string ip)
    {
        var ipAddress = IPAddress.Parse(ip);
        IPAddressExtensions.IsPrivate(ipAddress).Should().BeTrue();
    }

    [Theory]
    [InlineData("::1")]
    [InlineData("fe80::")]
    [InlineData("fe80:1234:5678::1")]
    [InlineData("fc00::")]
    [InlineData("fc00:1234:5678::1")]
    [InlineData("fd00::")]
    [InlineData("fd12:3456:789a::1")]
    public void IsPrivate_ReturnsTrue_PrivateIPv6(string ip)
    {
        var ipAddress = IPAddress.Parse(ip);
        IPAddressExtensions.IsPrivate(ipAddress).Should().BeTrue();
    }
}