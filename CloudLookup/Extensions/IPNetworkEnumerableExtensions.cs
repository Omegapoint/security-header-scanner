using System.Net;
using System.Net.Sockets;
using NetTools;

namespace CloudLookup.Extensions;

public static class IPNetworkEnumerableExtensions
{
    private static readonly AddressFamily[] SupportedFamilies =
        [AddressFamily.InterNetwork, AddressFamily.InterNetworkV6];
    
    public static IEnumerable<IPNetwork> MergeNeighbouring(this IEnumerable<IPNetwork> networks)
    {
        return SupportedFamilies.Aggregate(new List<IPNetwork>(), (all, family) =>
        {
            var filtered = networks.Where(n => n.BaseAddress.AddressFamily == family);
            
            all.AddRange(GetMergedNetworks(filtered));
            
            return all;
        });
    }

    private static IEnumerable<IPNetwork> GetMergedNetworks(IEnumerable<IPNetwork> networks)
    {
        var sorted = networks.OrderBy(n => n, new IPNetworkComparer())
            .Select(net => IPAddressRange.Parse(net.ToString()))
            .ToList();

        if (sorted.Count == 0)
        {
            return Array.Empty<IPNetwork>();
        }
        
        var result = new List<IPAddressRange>();

        var current = sorted.First();
        List<IPAddressRange> underlying = [current];
        var lastEnd = current.End;

        foreach (var range in sorted.Skip(1))
        {
            if (new IPAddressRange(current.Begin, lastEnd).Contains(range))
            {
                continue;
            }
            
            if (!range.Contains(lastEnd.Add(1)))
            {
                result.AddRange(MinimalCidrs(underlying));
                underlying = [];
            }
            
            underlying.Add(range);
            lastEnd = range.End;
        }
        
        result.AddRange(MinimalCidrs(underlying));

        return result.Select(range => range.ToIPNetwork());
    }

    private static IEnumerable<IPAddressRange> MinimalCidrs(IReadOnlyList<IPAddressRange> underlying)
    {
        if (underlying.Count == 0)
        {
            return [];
        }
        
        for (var size = underlying.Count; size > 0; size--)
        {
            for (var offset = 0; offset <= underlying.Count - size; offset++)
            {
                try
                {
                    var range = new IPAddressRange(underlying[offset].Begin, underlying[offset + size - 1].End);
                    
                    range.ToCidrString();

                    var pre = MinimalCidrs(underlying.Take(offset).ToList());
                    var post = MinimalCidrs(underlying.Skip(offset + size).ToList());
                    
                    return [..pre, range, ..post];
                }
                catch (Exception)
                {
                    // if range.ToCidrString fails, test next sliding window
                }
            }
        }

        throw new ArgumentException("MinimalCidrs should always return before loop completion");
    }
}