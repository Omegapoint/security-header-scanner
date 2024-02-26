using System.Net;
using System.Net.Http.Headers;
using headers.security.Common.Domain;
using headers.security.Common.Domain.SecurityConcepts;
using headers.security.Scanner.SecurityConcepts;
using HtmlAgilityPack;

namespace headers.security.Scanner;

public static class SecurityEngine
{
    private static readonly List<ISecurityConcept> Handlers = [
        StrictTransportSecurityConcept.Create(),
        XFrameOptionsSecurityConcept.Create(),
        XContentTypeOptionsSecurityConcept.Create(),
        CspSecurityConcept.Create(),
        PermissionsPolicySecurityConcept.Create(),
        ReferrerPolicySecurityConcept.Create(),
        
        // Non grade-influencing
        ServerSecurityConcept.Create(),
        AccessControlAllowOriginSecurityConcept.Create(), 
    ];
    
    public static async Task<ScanResult> Parse(HttpResponseMessage message)
    {
        var rawHeaders = ExtractHeaders(message.Headers);
        var rawHttpEquivMetas = await ExtractHttpEquivMetas(message.Content);
        
        var handlerResults = (await Task.WhenAll(
            Handlers.Select(handler => handler.ExecuteAsync(rawHeaders, rawHttpEquivMetas, message))
        )).Where(r => r != null);

        return new ScanResult(handlerResults.OrderBy(r => r.HandlerName), rawHeaders);
    }

    private static RawHeaders ExtractHeaders(HttpResponseHeaders headers)
        => new(headers);

    private static async Task<RawHeaders> ExtractHttpEquivMetas(HttpContent messageContent)
    {
        var doc = new HtmlDocument();
        doc.Load(await messageContent.ReadAsStreamAsync());

        var metas = (doc.DocumentNode
            ?.SelectNodes("//html/head")
            ?.SelectMany(head => head.SelectNodes("//meta")) ?? [])
            .Where(node => node.HasAttributes);

        var httpEquivMetas = metas
            // TODO: these .Contains operations may need to ignore case? they may already, test
            .Where(node => node.Attributes.Contains("http-equiv") && node.Attributes.Contains("content"));

        return new(httpEquivMetas.GroupBy(
            httpEquivMeta => httpEquivMeta.Attributes["http-equiv"].Value,
            httpEquivMeta => httpEquivMeta.Attributes["content"].Value,
            StringComparer.OrdinalIgnoreCase
        ));
    }

    public static void ExamineNonceUsage((IPAddress IP, Uri FinalUri, DateTime FetchedAt, ScanResult ScanResult)[] uriResults)
    { 
        var cspResults = uriResults
            .Select(kvp => (
                ID: Guid.NewGuid(),
                CSP: kvp.ScanResult.HandlerResults
                    .SingleOrDefault(hr => hr is CspSecurityConceptResult) as CspSecurityConceptResult
            ))
            .Where(kvp => kvp.CSP != null)
            .ToList();

        var cspDict = cspResults
            .ToDictionary(kvp => kvp.ID, kvp => kvp.CSP);
        
        var nonceSets = cspResults
            .Select(kvp => (kvp.ID, Nonces: kvp.CSP.ProcessedValue.GetNonces()))
            .ToList();

        var reused = new Dictionary<Guid, HashSet<string>>();
        List<Guid> seen = [];
        
        foreach (var (id, nonceSet) in nonceSets)
        {
            seen.Add(id);
            foreach (var (otherId, otherSet) in nonceSets)
            {
                if (seen.Contains(otherId)) continue;
                
                HashSet<string> nonces = [..nonceSet];
                nonces.IntersectWith(otherSet);

                if (nonces.Count == 0) continue;
                
                if (!reused.TryAdd(id, nonces))
                {
                    reused[id].UnionWith(nonces);
                }

                if (!reused.TryAdd(otherId, nonces))
                {
                    reused[otherId].UnionWith(nonces);
                }
            }
        }

        foreach (var (id, nonces) in reused)
        {
            var result = cspDict[id];
            
            var noncePart = nonces.Count > 1 ? "nonces were" : "nonce was";
            
            var message = $"Nonce re-use detected. The following {noncePart} detected in multiple responses from the server: {string.Join(", ", nonces)}";

            result.Infos.Add(SecurityConceptResultInfo.Create(message));

            result.NonceReuse = true;
        }
    }
}