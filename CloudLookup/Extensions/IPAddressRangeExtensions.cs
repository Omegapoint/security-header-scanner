using System.Net;
using NetTools;

namespace CloudLookup.Extensions;

public static class IPAddressRangeExtensions
{
    public static IPNetwork ToIPNetwork(this IPAddressRange range)
        => IPNetwork.Parse(range.ToCidrString());
}