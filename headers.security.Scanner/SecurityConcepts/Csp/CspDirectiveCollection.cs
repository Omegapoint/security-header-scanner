using headers.security.Common;
using static headers.security.Common.Constants.Csp.CspDirective;

namespace headers.security.Scanner.SecurityConcepts.Csp;

/// <summary>
/// A CSP directive collection, always returns a value, but does not mutate the collection when keys are missing.
/// </summary>
public class CspDirectiveCollection() 
    : DefaultDictionary<string, HashSet<string>>(() => [], StringComparer.OrdinalIgnoreCase)
{
    public new HashSet<string> this[string key] {
        get => ContainsKey(key) 
            ? base[key] 
            : FallbackOrDefault(key);
        set => base[key] = value;
    }

    private HashSet<string> FallbackOrDefault(string key)
    {
        var fallbackKey = GetFallbackKey(key);

        return !string.IsNullOrEmpty(fallbackKey) && ContainsKey(fallbackKey)
            ? base[fallbackKey]
            : [];
    }
    
    private static string GetFallbackKey(string key) => FallsBackToDefaultSrc(key)
        ? DefaultSrc
        : null;
    
    public override string ToString() => string.Join("; ", this.Select(kvp => $"{kvp.Key} {string.Join(' ', kvp.Value)}"));
}