using System.Net;
using System.Net.Sockets;
using System.Numerics;

namespace CloudLookup.Extensions;

public static class IPAddressExtensions
{
    public static IPAddress Subtract(this IPAddress originalAddress, BigInteger offset)
        => originalAddress.Add(-offset);

    public static IPAddress Add(this IPAddress originalAddress, BigInteger offset)
    {
        if (originalAddress.AddressFamily == AddressFamily.InterNetwork)
        {
            originalAddress = originalAddress.MapToIPv6();
        }
        
        var numerical = originalAddress.ToBigInteger() + offset;
        var newAddress = numerical.ToIPAddress();

        return newAddress.IsIPv4MappedToIPv6 ? newAddress.MapToIPv4() : newAddress;
    }

    private static BigInteger ToBigInteger(this IPAddress address)
        => new(address.GetAddressBytes(), isBigEndian: true, isUnsigned: true);
}