using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Text.Json.Serialization;
using headers.security.Scanner.Hsts.Contracts;

namespace headers.security.Scanner.Hsts;

public class PreloadPolicyNode()
{
    public PreloadPolicy Policy { get; set; }

    public IDictionary<string, PreloadPolicyNode> Nodes { get; } = new Dictionary<string, PreloadPolicyNode>();

    [JsonConstructor]
    public PreloadPolicyNode(PreloadPolicy policy, IDictionary<string, PreloadPolicyNode> nodes)
        : this(policy, nodes.ToFrozenDictionary())
    {
    }

    private PreloadPolicyNode(PreloadPolicy policy, FrozenDictionary<string, PreloadPolicyNode> nodes) : this()
    {
        Policy = policy;
        Nodes = nodes;
    }

    private PreloadPolicyNode Freeze() => new(
        Policy,
        Nodes.ToFrozenDictionary(kvp => kvp.Key, kvp => kvp.Value.Freeze())
    );

    public static PreloadPolicyNode Create(IEnumerable<PreloadEntry> entries)
    {
        var root = new PreloadPolicyNode();
        
        foreach (var preloadEntry in entries)
        {
            root.Set(preloadEntry);
        }

        return root.Freeze();
    }

    public PreloadPolicy GetOrDefault(string domain) => string.IsNullOrWhiteSpace(domain) 
        ? null 
        : GetOrDefault(domain.ToLowerInvariant().Split('.').Reverse().ToImmutableArray(), 0);

    private PreloadPolicy GetOrDefault(ImmutableArray<string> key, int depth)
    {
        // exact match at deepest node
        if (depth == key.Length)
        {
            return Policy;
        }

        // depth first recursion
        if (Nodes.TryGetValue(key[depth], out var node))
        {
            var policy = node.GetOrDefault(key, depth + 1);

            if (policy != null)
            {
                return policy;
            }
        }

        // if no policy found at deeper level, but current level includes subdomains, return current level policy
        return Policy?.IncludeSubdomains == true ? Policy : null;
    }

    /// <summary>
    /// Sets the <see cref="PreloadPolicy"/> for the given <see cref="PreloadEntry"/>, overwrites if it already exists.
    /// </summary>
    private void Set(PreloadEntry entry) => Set(entry, 0);

    private void Set(PreloadEntry entry, int depth)
    {
        if (depth == entry.TreePath.Length)
        {
            Policy = entry;

            return;
        }

        var label = entry.TreePath[depth];
        
        if (!Nodes.TryGetValue(label, out var node))
        {
            node = new PreloadPolicyNode();
            Nodes.Add(label, node);
        }
            
        node.Set(entry, depth + 1);
    }
}