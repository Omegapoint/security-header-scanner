using System.Net;
using System.Net.Sockets;

namespace headers.security.Common.Extensions;

// ReSharper disable once InconsistentNaming
public static class IPAddressExtensions
{
    /// <summary>
    /// Returns true if the IP address is in a private range.
    /// </summary>
    /// <param name="ip">The IP address.</param>
    public static bool IsPrivate(this IPAddress ip)
    {
        if (ip.IsIPv4MappedToIPv6)
        {
            ip = ip.MapToIPv4();
        }

        // Checks loopback ranges for both IPv4 and IPv6.
        if (IPAddress.IsLoopback(ip))
        {
            return true;
        }

        return ip.AddressFamily switch
        {
            // IPv4
            AddressFamily.InterNetwork => IsPrivateIPv4(ip.GetAddressBytes()),
            // IPv6
            AddressFamily.InterNetworkV6 => ip.IsIPv6LinkLocal || ip.IsIPv6UniqueLocal || ip.IsIPv6SiteLocal,
            _ => throw new NotSupportedException($"IP address family {ip.AddressFamily} is not supported.")
        };
    }

    /// <summary>
    /// Returns true if the IPv4 represented as a byte array is in any block reserved private.
    /// </summary>
    /// <param name="ipv4Bytes">The IP address represented as a byte array.</param>
    private static bool IsPrivateIPv4(IReadOnlyList<byte> ipv4Bytes) =>
        ipv4Bytes[0] switch
        {
            // Class A private range 10/8
            10 => true,
            // CGNat range 100.64/10
            100 => ipv4Bytes[1] >= 64 && ipv4Bytes[1] <= 127,
            // Link local 169.254/16
            169 => ipv4Bytes[1] == 254,
            // Class B private range 172.16/12
            172 => ipv4Bytes[1] >= 16 && ipv4Bytes[1] <= 31,
            // Class C private range 192.168/16
            192 => ipv4Bytes[1] == 168,
            _ => false
        };
}