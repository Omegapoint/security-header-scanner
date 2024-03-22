using System.Collections.ObjectModel;
using System.Net.Http.Headers;

namespace headers.security.Common.Domain;

public class RawHeaders : ReadOnlyDictionary<string, List<string>>
{
    public RawHeaders(params HttpHeaders[] headerBags)
        : base(headerBags.SelectMany(h => h)
            .ToLookup(p => p.Key, p => p.Value.ToList(), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(p => p.Key, p => p.SelectMany(v => v).ToList(), StringComparer.OrdinalIgnoreCase)
            .AsReadOnly())
    {
    }

    public RawHeaders(IEnumerable<IGrouping<string, string>> headerGroup)
        : base(headerGroup.ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase).AsReadOnly())
    {
    }
}