using headers.security.Common.Domain;
using headers.security.Common.Domain.SecurityConcepts;

namespace headers.security.Scanner.SecurityConcepts;

public interface ISecurityConcept
{
    Task<ISecurityConceptResult> ExecuteAsync(CrawlerConfiguration crawlerConf, RawHeaders rawHeaders, RawHeaders rawHttpEquivMetas, HttpResponseMessage message);
}