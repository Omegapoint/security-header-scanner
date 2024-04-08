using headers.security.Common.Constants;
using headers.security.Common.Domain;
using Microsoft.Net.Http.Headers;
using static headers.security.Common.Constants.CspDirective;

namespace headers.security.Scanner.SecurityConcepts.Csp;

public static class CspParser
{
    public const string NoncePrefix = "'nonce-";
    
    public static CspConfiguration ExtractAll(RawHeaders rawHeaders, RawHeaders rawHttpEquivMetas)
    {
        var enforcingPolicies = ExtractPolicies(HeaderNames.ContentSecurityPolicy, true);
        var nonEnforcingPolicies = ExtractPolicies(HeaderNames.ContentSecurityPolicyReportOnly, false);

        var allPolicies = enforcingPolicies.Concat(nonEnforcingPolicies);

        return new CspConfiguration(
            enforcingPolicies,
            nonEnforcingPolicies,
            GetEffectivePolicy(allPolicies.ToList())
        );

        List<CspPolicy> ExtractPolicies(string headerName, bool enforcing)
        {
            var rawPolicies = new List<(string Source, string Policy)>();
        
            if (rawHeaders.TryGetValue(headerName, out var headerPolicies))
            {
                rawPolicies.AddRange(headerPolicies.Select(policy => ("Header", policy)));
            }
        
            if (rawHttpEquivMetas.TryGetValue(headerName, out var httpEquivMetas))
            {
                rawPolicies.AddRange(httpEquivMetas.Select(policy => ("HttpEquiv", policy)));
            }
        
            return rawPolicies
                .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Policy))
                .Select(kvp => new CspPolicy(kvp.Source, kvp.Policy, enforcing))
                .ToList();
        }
    }

    public static CspPolicy GetEffectivePolicy(List<CspPolicy> allPolicies)
    {
        if (allPolicies.Count == 0)
        {
            return null;
        }

        var isEnforcing = allPolicies.Any(p => p.Enforcing);
        var policies = allPolicies.Where(p => p.Enforcing == isEnforcing).ToList();
        
        var defaultSrcValue = GetMostStrict(DefaultSrc);

        var directives = new CspDirectiveCollection();
        
        if (defaultSrcValue != null)
        {
            directives[DefaultSrc] = defaultSrcValue;
        }

        var directiveNames = policies.SelectMany(p => p.Directives.Keys).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var directiveName in directiveNames)
        {
            if (directiveName is DefaultSrc)
            {
                continue;
            }

            var useDefaultSrcFallback = defaultSrcValue != null
                && FallsBackToDefaultSrc(directiveName);

            var fallback = useDefaultSrcFallback && !policies.All(p => p.Directives.ContainsKey(directiveName)) ? defaultSrcValue : null;

            var values = GetMostStrict(directiveName, fallback);
            
            // don't add to directives if identical to default-src
            if (useDefaultSrcFallback && values?.SetEquals(defaultSrcValue) == true)
            {
                continue;
            }

            directives[directiveName] = values;
        }

        return new CspPolicy("Merged", directives, isEnforcing);

        HashSet<string> GetMostStrict(string directive, HashSet<string> fallback = null)
        {
            HashSet<string> accumulator = null;
            if (fallback != null)
            {
                accumulator = new(fallback, StringComparer.OrdinalIgnoreCase);
            }

            foreach (var policy in policies)
            {
                if (!policy.Directives.TryGetValue(directive, out var policyTokens))
                {
                    continue;
                }

                if (accumulator == null || accumulator.Contains("*") && !policyTokens.Contains("*"))
                {
                    accumulator = new(policyTokens, StringComparer.OrdinalIgnoreCase);
                }
                else if (accumulator.Contains("*") && policyTokens.Contains("*"))
                {
                    accumulator.UnionWith(policyTokens);
                }
                else
                {
                    if (!policyTokens.Contains("*"))
                    {
                        accumulator.IntersectWith(policyTokens);
                    }
                }
            }

            if (accumulator?.Count == 0)
            {
                accumulator.Add(CspToken.None);
            }

            return accumulator;
        }
    }
}