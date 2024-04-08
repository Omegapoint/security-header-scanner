using headers.security.Common;

namespace headers.security.Scanner.SecurityConcepts.Csp;

public class CspConfiguration(List<CspPolicy> all, List<CspPolicy> allNonEnforcing, CspPolicy effective)
{
    [Compare]
    public List<CspPolicy> All { get; } = all;
    
    [Compare]
    public List<CspPolicy> AllNonEnforcing { get; } = allNonEnforcing;
    
    public CspPolicy Effective { get; } = effective;
    
    public bool HasPolicy => All.Count > 0 || AllNonEnforcing.Count > 0;

    public bool IsEnforcing => HasPolicy && Effective.Enforcing;

    public HashSet<string> ExtractNonces()
    {
        List<CspPolicy> policies = [..All, ..AllNonEnforcing];
        return policies.Aggregate(new HashSet<string>(), (nonces, policy) =>
        {
            nonces.UnionWith(policy.ExtractNonces());
            return nonces;
        });
    }
}