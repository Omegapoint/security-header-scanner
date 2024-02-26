using headers.security.Common;

namespace headers.security.Scanner.SecurityConcepts.Csp;

public class CspConfiguration(List<CspPolicy> all, List<CspPolicy> allNonEnforcing, CspPolicy effective)
{
    [Compare]
    public List<CspPolicy> All { get; } = all;
    
    [Compare]
    public List<CspPolicy> AllNonEnforcing { get; } = allNonEnforcing;
    
    // TODO: should this ever contain a non-enforcing policy? if there are only non-enforcing policies maybe?
    public CspPolicy Effective { get; } = effective;
    
    public bool HasPolicy => All.Count > 0;

    public HashSet<string> GetNonces()
    {
        List<CspPolicy> policies = [..All, ..AllNonEnforcing];
        return policies.Aggregate(new HashSet<string>(), (nonces, policy) =>
        {
            nonces.UnionWith(policy.GetNonces());
            return nonces;
        });
    }
}