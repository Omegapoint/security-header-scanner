using System.Net;
using headers.security.Common.Domain;
using headers.security.Common.Domain.SecurityConcepts;
using headers.security.Scanner.Extensions;
using headers.security.Scanner.SecurityConcepts;
using HtmlAgilityPack;

namespace headers.security.Scanner;

public class SecurityEngine(List<ISecurityConcept> handlers)
{
    private const int MaxAllowedContentSize = 1024 * 2000;

    public async Task<ScanResult> Parse(Uri target, HttpResponseMessage message,
        CrawlerConfiguration crawlerConfiguration)
    {
        var rawHeaders = ExtractHeaders(message);
        var scanData = new ScanData(message, target, crawlerConfiguration, rawHeaders, await ExtractHttpEquivMetas(message));
        
        var handlerResults = (await Task.WhenAll(
            handlers.Select(handler => handler.ExecuteAsync(scanData))
        )).Where(r => r != null);

        return new ScanResult(handlerResults.OrderBy(r => r.HandlerName), rawHeaders, scanData.TargetType);
    }

    private RawHeaders ExtractHeaders(HttpResponseMessage message)
        => new(message.Headers, message.Content.Headers);

    private async Task<RawHeaders> ExtractHttpEquivMetas(HttpResponseMessage message)
    {
        if (!message.LooksLikeFrontendResponse())
        {
            return new();
        }

        var maybeContent = await message.Content.ReadAtMostAsync(MaxAllowedContentSize);
        
        var doc = new HtmlDocument();
        doc.LoadHtml(maybeContent);

        var heads = doc.DocumentNode.SelectNodes("//html/head");

        var httpEquivMetas = new List<HtmlNode>();

        foreach (var head in heads)
        {
            var htmlNodes = head.SelectNodes("//meta");
            if (htmlNodes == null)
            {
                continue;
            }
            
            httpEquivMetas.AddRange(htmlNodes
                .Where(node => node.HasAttributes)
                .Where(node => node.Attributes.Contains("http-equiv") && node.Attributes.Contains("content")));
        }

        return new(httpEquivMetas.GroupBy(
            httpEquivMeta => httpEquivMeta.Attributes["http-equiv"].Value,
            httpEquivMeta => httpEquivMeta.Attributes["content"].Value,
            StringComparer.OrdinalIgnoreCase
        ));
    }

    public void ExamineNonceUsage((IPAddress IP, Uri FinalUri, DateTime FetchedAt, ScanResult ScanResult)[] uriResults)
    { 
        var cspResults = uriResults
            .Select(kvp => (
                CSPID: Guid.NewGuid(),
                CSP: kvp.ScanResult.HandlerResults
                    .SingleOrDefault(hr => hr is CspSecurityConceptResult) as CspSecurityConceptResult
            ))
            .Where(kvp => kvp.CSP != null)
            .ToArray();

        var cspDict = cspResults
            .ToDictionary(kvp => kvp.CSPID, kvp => kvp.CSP);
        
        var reusedNonces = FindReusedNonces(cspResults);

        foreach (var (id, nonces) in reusedNonces)
        {
            cspDict[id].Infos.Add(CspNonceSecurityConceptResultInfo.Reuse(nonces));
        }
    }

    private Dictionary<Guid, HashSet<string>> FindReusedNonces((Guid CSPID, CspSecurityConceptResult CSP)[] cspResults)
    {
        var reused = new Dictionary<Guid, HashSet<string>>();
        
        var nonceSets = cspResults
            .Select(kvp => (kvp.CSPID, Nonces: kvp.CSP.ProcessedValue.ExtractNonces()))
            .ToList();

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

        return reused;
    }
}