using headers.security.Common;
using headers.security.Common.Domain;
using headers.security.Common.Extensions;

namespace headers.security.Scanner;

public class Worker(Crawler crawler)
{
    public Task<List<ServerResult>> PerformScan(CrawlerConfiguration crawlerConf, List<Uri> uris)
        => PerformScan(crawlerConf, uris.ToArray());
    
    public async Task<List<ServerResult>> PerformScan(CrawlerConfiguration crawlerConf, params Uri[] uris)
    {
        if (uris.Length > 256)
        {
            throw new GenericScannerException("Can only scan a maximum of 256 URIs at one time.", ErrorOrigin.SystemLimitation);
        }

        var asciiUris = uris.Select(uri => uri.IdnNormalized()).ToList();
        
        var results = new List<ServerResult>();

        foreach (var targetUri in asciiUris)
        {
            try
            {
                var crawlerResponses = await crawler.Crawl(targetUri, crawlerConf);

                var successful = crawlerResponses.Where(response => !response.IsFailure).ToList();
                if (successful.Count != 0)
                {
                    var uriResults = await Task.WhenAll(
                        successful.Select(async response => (
                            response.IP,
                            response.FinalUri,
                            response.FetchedAt,
                            await SecurityEngine.Parse(response.HttpMessage, crawlerConf)
                        ))
                    );
                    SecurityEngine.ExamineNonceUsage(uriResults);
                    results.AddRange(ServerResultComparer.MergeEqual(uriResults.ToList(), targetUri));
                }
                
                var failures = crawlerResponses.Where(response => response.IsFailure).ToList();
                if (failures.Count != 0)
                {
                    results.AddRange(failures.Select(response => new ServerResult
                    {
                        RequestTarget = ScanTarget.From(targetUri),
                        FinalTarget = ScanTarget.From(response.FinalUri),
                        IPs = [response.IP.ToString()],
                        Error = response.ScanError,
                        FetchedAt = response.FetchedAt
                    }));
                }
            }
            catch (ScannerException e)
            {
                results.Add(new ServerResult
                {
                    RequestTarget = ScanTarget.From(targetUri),
                    FinalTarget = ScanTarget.From(targetUri),
                    Error = e.ToContract()
                });
            }
        }
        
        return results;
    }
}