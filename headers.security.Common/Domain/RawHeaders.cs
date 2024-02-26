using System.Net.Http.Headers;

namespace headers.security.Common.Domain;

public class RawHeaders(IDictionary<string, List<string>> headers) : Dictionary<string, List<string>>(headers)
{
    public RawHeaders(HttpResponseHeaders headers)
        : this(headers.ToDictionary(p => p.Key, p => p.Value.ToList(), StringComparer.OrdinalIgnoreCase))
    {
    }

    public RawHeaders(IEnumerable<IGrouping<string, string>> headerGroup)
        : this(headerGroup.ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase))
    {
    }
}