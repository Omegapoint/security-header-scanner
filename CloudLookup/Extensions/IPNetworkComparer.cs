using System.Net;

namespace CloudLookup.Extensions;

public class IPNetworkComparer : IComparer<IPNetwork>
{
    public int Compare(IPNetwork x, IPNetwork y)
    {
        var xBytes = x.BaseAddress.GetAddressBytes();
        var yBytes = y.BaseAddress.GetAddressBytes();
        
        foreach (var (xByte, yByte) in xBytes.Zip(yBytes))
        {
            var result = xByte.CompareTo(yByte);
            if (result != 0)
            {
                return result;
            }
        }
        
        return x.PrefixLength.CompareTo(y.PrefixLength);
    }
}