using System.Collections.Frozen;
using System.Net;
using System.Text.Json.Serialization;
using CloudLookup.Extensions;

namespace CloudLookup;

public class CloudLookupCollection
{
    public IDictionary<Cloud, IEnumerable<string>> Data { get; private set; }
    
    private readonly FrozenDictionary<Cloud, FrozenSet<IPNetwork>> _networksByCloud;

    private readonly FrozenDictionary<IPNetwork, Cloud> _cloudByNetwork;

    [JsonConstructor]
    public CloudLookupCollection(IDictionary<Cloud, IEnumerable<string>> data)
    {
        _networksByCloud = data.Where(kvp => kvp.Value != null)
            .ToFrozenDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Select(IPNetwork.Parse).ToFrozenSet()
            );
        _cloudByNetwork = ReverseLookup(_networksByCloud);

        Data = data;
    }

    public CloudLookupCollection(IEnumerable<(Cloud Cloud, IEnumerable<IPNetwork> Networks)> results)
    {
        _networksByCloud = results.Where(kvp => kvp.Networks != null)
            .ToFrozenDictionary(
                kvp => kvp.Cloud,
                kvp => kvp.Networks.MergeNeighbouring().ToFrozenSet()
            );
        _cloudByNetwork = ReverseLookup(_networksByCloud);

        Data = _networksByCloud.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Select(n => n.ToString()));
    }

    private FrozenDictionary<IPNetwork, Cloud> ReverseLookup(IDictionary<Cloud, FrozenSet<IPNetwork>> networksByCloud)
    {
        var reversedDictionary = new Dictionary<IPNetwork, Cloud>();

        foreach (var (cloud, networks) in networksByCloud)
        {
            foreach (var network in networks)
            {
                reversedDictionary[network] = cloud;
            }
        }

        return reversedDictionary.ToFrozenDictionary();
    }

    public Cloud GetCloud(IPAddress address)
    {
        foreach (var (network, cloud) in _cloudByNetwork)
        {
            if (network.Contains(address))
            {
                return cloud;
            }
        }

        return Cloud.Unknown;
    }

    public IEnumerable<IPNetwork> GetNetworks(Cloud cloud)
    {
        if (_networksByCloud.TryGetValue(cloud, out var ranges))
        {
            return ranges;
        }

        throw new ArgumentException($"Unknown Cloud {cloud}");
    }
}