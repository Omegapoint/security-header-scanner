using headers.security.Common;
using headers.security.Common.Constants;
using static headers.security.Common.Constants.CspDirective;
using static headers.security.Common.Constants.CspToken;

namespace headers.security.Scanner.SecurityConcepts.Csp;

public class CspPolicy
{
    [Compare]
    public string Source { get; set; }
    
    public bool Enforcing { get; init; }

    [Compare]
    public CspDirectiveCollection Directives { get; } = new();
    
    public CspPolicy(string source, string policy, bool enforcing)
    {
        Source = source;
        Enforcing = enforcing;
        
        foreach (var directiveWithValue in policy.Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
        {
            var splitDirectiveWithValue = directiveWithValue.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            
            var directive = splitDirectiveWithValue.First().ToLowerInvariant();
            var tokens = splitDirectiveWithValue.Skip(1);
            
            Directives[directive] = tokens.ToHashSet(StringComparer.OrdinalIgnoreCase);
        }
    }

    public CspPolicy(string source, CspDirectiveCollection directives, bool enforcing)
    {
        Source = source;
        Directives = directives;
        Enforcing = enforcing;
    }

    public HashSet<string> ExtractNonces() => Directives.Values
        .SelectMany(values => values.Where(v => v.StartsWith(CspParser.NoncePrefix)))
        .Select(nonce => nonce[7..^1])
        .ToHashSet();

    public bool IsUnsafe => GetUnsafeTokens().Any();

    public IEnumerable<string> GetUnsafeTokens()
    {
        var tokens = GetScriptTokens();

        tokens.IntersectWith([
            UnsafeInline,
            UnsafeEval
        ]);

        return tokens;
    }

    public IEnumerable<string> GetBypassTokens() =>
        GetScriptTokens().Where(KnownCspBypassUris.Matches);

    private HashSet<string> GetScriptTokens() =>
    [
        ..Directives[ScriptSrc],
        ..Directives[ScriptSrcElem],
        ..Directives[ScriptSrcAttr]
    ];
}