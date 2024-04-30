using System.Net;
using System.Numerics;

namespace CloudLookup.Extensions;

public static class BigIntegerExtensions
{
    public static IPAddress ToIPAddress(this BigInteger address)
    {
        var bytes = address.ToByteArray(isBigEndian: true, isUnsigned: true);
        if (bytes.Length < 16)
        {
            bytes = new byte[16 - bytes.Length].Concat(bytes).ToArray();
        }
        
        return new IPAddress(bytes);
    }
}