using headers.security.Common;

namespace headers.security.Scanner.SecurityConcepts.Csp;

public class CspPolicy
{
    [Compare]
    public string Source { get; set; }
    
    public bool Enforcing { get; init; }

    [Compare]
    public Dictionary<string, HashSet<string>> Directives { get; } = new(StringComparer.OrdinalIgnoreCase);
    
    public CspPolicy(string source, string policy, bool enforcing)
    {
        Source = source;
        Enforcing = enforcing;
        
        foreach (var directiveWithValue in policy.Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
        {
            var tokens = directiveWithValue.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var directive = tokens.First().ToLowerInvariant();
            
            Directives[directive] = tokens.Skip(1).ToHashSet(StringComparer.OrdinalIgnoreCase);
        }
    }

    public CspPolicy(string source, Dictionary<string, HashSet<string>> directives, bool enforcing)
    {
        Source = source;
        Directives = directives;
        Enforcing = enforcing;
    }

    public HashSet<string> GetNonces() => Directives
        .SelectMany(kvp => kvp.Value.Where(v => v.StartsWith(CspParser.NoncePrefix)))
        .ToHashSet();
}