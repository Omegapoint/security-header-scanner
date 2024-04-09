using headers.security.Common.Constants;
using headers.security.Common.Domain;
using headers.security.Common.Domain.SecurityConcepts;
using headers.security.Scanner.SecurityConcepts.Csp;
using Microsoft.Net.Http.Headers;

namespace headers.security.Scanner.SecurityConcepts;

public class CspSecurityConcept : ISecurityConcept
{
    public const string HandlerName = "Content Security Policy";

    public static ISecurityConcept Create() => new CspSecurityConcept();

    public Task<ISecurityConceptResult> ExecuteAsync(
        CrawlerConfiguration crawlerConf,
        RawHeaders rawHeaders,
        RawHeaders rawHttpEquivMetas,
        HttpResponseMessage message) 
        => Task.FromResult(Execute(crawlerConf, rawHeaders, rawHttpEquivMetas, message));

    private ISecurityConceptResult Execute(
        CrawlerConfiguration crawlerConf,
        RawHeaders rawHeaders,
        RawHeaders rawHttpEquivMetas,
        HttpResponseMessage message)
    {
        var infos = new List<ISecurityConceptResultInfo>();

        var configuration = CspParser.ExtractAll(rawHeaders, rawHttpEquivMetas);

        if (configuration.All.Count > 1)
        {
            infos.Add(SecurityConceptResultInfo.Create("Multiple policies present."));
        }

        if (configuration.All.Count == 0 && configuration.AllNonEnforcing.Count >= 1)
        {
            infos.Add(SecurityConceptResultInfo.Create("Policy is not enforcing."));
        }

        if (!configuration.HasPolicy)
        {
            infos.Add(SecurityConceptResultInfo.Create("No policy present, consider adding one."));
        }
        else
        {
            var effective = configuration.Effective;

            if (effective.Directives.TryGetValue(CspDirective.Referrer, out _))
            {
                infos.Add(SecurityConceptResultInfo.Create($"The \"{CspDirective.Referrer}\" directive of the CSP header is obsolete and should be removed."));
            }
            
            var unsafeTokens = effective.GetUnsafeTokens().ToList();
            if (unsafeTokens.Count != 0)
            {
                infos.Add(new SecurityConceptResultInfo {
                    Message = "Policy allows unsafe practices ({0}) in scripts. This may be necessary in order to use some JavaScript frameworks, but substantially reduces the policies effectiveness at stopping cross-site scripting.",
                    FormatTokens = [unsafeTokens]
                });
            }

            var bypassTokens = effective.GetBypassTokens().ToList();
            if (bypassTokens.Count != 0)
            {
                infos.Add(new SecurityConceptResultInfo {
                    Message = "Policy allows known CSP bypass targets ({0}) in scripts.",
                    FormatTokens = [bypassTokens]
                });
            }
        }
        
        var lowComplexityNonce = configuration.ExtractNonces().FirstOrDefault(n => n.Length < 22);
        if (lowComplexityNonce != null)
        {
            infos.Add(CspNonceSecurityConceptResultInfo.LowComplexity(lowComplexityNonce));
        }
        
        // TODO: FUTURE: data exfil: img-src, style-src, connect-src, big cloud providers with wildcards, etc. -> info
        
        // TODO: no form-action specified -> info, link: https://portswigger.net/research/using-form-hijacking-to-bypass-csp

        return new CspSecurityConceptResult(configuration, infos);
    }
}

public class CspSecurityConceptResult(CspConfiguration configuration, List<ISecurityConceptResultInfo> infos)
    : AbstractSecurityConceptResult(infos)
{
    public override string HandlerName => CspSecurityConcept.HandlerName;
    public override string HeaderName => HeaderNames.ContentSecurityPolicy;

    public override SecurityImpact Impact => GetImpact();
    public override CspConfiguration ProcessedValue => configuration;

    public bool NonceIssue => Infos.Any(i => i is CspNonceSecurityConceptResultInfo);

    private SecurityImpact GetImpact()
    {
        if (NonceIssue)
        {
            return SecurityImpact.Critical;
        }
        
        if (!configuration.HasPolicy)
        {
            return SecurityImpact.Medium;
        }

        if (configuration.Effective.IsUnsafe)
        {
            return SecurityImpact.Low;
        }

        return SecurityImpact.None;
    }
}