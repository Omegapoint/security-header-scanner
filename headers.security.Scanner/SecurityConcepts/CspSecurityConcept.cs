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
        
        // todo: if we encounter unsafe-eval and/or unsafe-inline in script-src then at least low
        // todo: we could inform that might be because of framework used
        
        // todo: known csp-bypasses lowers score
        
        var lowComplexityNonce = configuration.ExtractNonces().FirstOrDefault(n => n.Length < 22);
        if (lowComplexityNonce != null)
        {
            infos.Add(CspNonceSecurityConceptResultInfo.LowComplexity(lowComplexityNonce));
        }
        
        // TODO: data exfil?? img-src, style-src, connect-src, etc. -> info
        
        // todo: no form-action specified -> info, link: https://portswigger.net/research/using-form-hijacking-to-bypass-csp

        return new CspSecurityConceptResult(configuration, infos);
    }
}

public class CspSecurityConceptResult(CspConfiguration configuration, List<ISecurityConceptResultInfo> infos) : AbstractSecurityConceptResult(infos)
{
    public override string HandlerName => CspSecurityConcept.HandlerName;
    public override string HeaderName => HeaderNames.ContentSecurityPolicy;

    public override SecurityImpact Impact => GetImpact();
    public override CspConfiguration ProcessedValue => configuration;

    public bool NonceIssue => Infos.Any(i => i is CspNonceSecurityConceptResultInfo);

    private SecurityImpact GetImpact()
    {
        if (!configuration.HasPolicy)
        {
            return SecurityImpact.Medium;
        }
        
        if (NonceIssue)
        {
            return SecurityImpact.Critical;
        }
        
        //TODO: more complex logic for grading?

        return SecurityImpact.None;
    }
}