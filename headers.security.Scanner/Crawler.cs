using System.Net;
using System.Net.Sockets;
using headers.security.Common;
using headers.security.Common.Domain;
using headers.security.Common.Extensions;
using headers.security.Scanner.Extensions;

namespace headers.security.Scanner;

public class Crawler(WorkerConfiguration workerConf, IHttpClientFactory httpClientFactory) : ICrawler
{
    public async Task<List<CrawlerResponse>> Crawl(Uri uri, CrawlerConfiguration crawlerConf)
    {
        if (!workerConf.AllowInternalTargets)
        {
            await EnsureExternalTarget(uri, crawlerConf.CancellationToken);
        }
        
        var hostsToScan = await ResolveHost(uri, crawlerConf.CancellationToken);
        
        if (hostsToScan.Count == 1)
        {
            hostsToScan.Add(hostsToScan.Single());
        }

        var responses = await Task.WhenAll(
            hostsToScan.Select(ipAddress => GetResponse(uri, ipAddress, crawlerConf))
        );

        return responses.ToList();
    }

    private async Task<CrawlerResponse> GetResponse(Uri uri, IPAddress ipAddress, CrawlerConfiguration crawlerConf)
    {
        var client = httpClientFactory.CreateClient("Scanner");

        var response = new CrawlerResponse
        {
            IP = ipAddress
        };
        
        var currentUri = uri;
        var request = currentUri.MakeRequestToIp(ipAddress, HttpMethod.Get);

        try
        {
            var httpResponse = await client.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                crawlerConf.CancellationToken);

            httpResponse.EnsureNotResponseFromSelf();
            
            response.FetchedAt = DateTime.UtcNow;

            var requestDepth = 0;
            if (crawlerConf.FollowRedirects)
            {
                while (requestDepth < workerConf.MaxHttpRequestDepth && httpResponse.IsRedirectStatusCode())
                {
                    currentUri = httpResponse.GetAbsoluteRedirectUri(currentUri);
                    if (currentUri == null || !await IsValidRedirect(currentUri, crawlerConf.CancellationToken))
                    {
                        break;
                    }

                    var redirectRequest = new HttpRequestMessage(HttpMethod.Get, currentUri);

                    httpResponse = await client.SendAsync(
                        redirectRequest,
                        HttpCompletionOption.ResponseHeadersRead,
                        crawlerConf.CancellationToken);
                    
                    httpResponse.EnsureNotResponseFromSelf();

                    response.FetchedAt = DateTime.UtcNow;

                    requestDepth += 1;
                }
            }

            response.HttpMessage = httpResponse;
        }
        catch (ScannerException e)
        {
            response.ScanError = e.ToContract();
        } 
        catch (Exception e)
        {
            var failureReason = e switch
            {
                HttpRequestException => "Failed to connect to server.",
                TaskCanceledException => "Server did not respond in allotted time.",
                _ => "Unrecoverable error"
            };

            response.ScanError = new ScanError
            {
                Message = failureReason,
                Origin = ErrorOrigin.Other
            };
        }
        
        response.FinalUri = currentUri;
        
        return response;
    }

    private async Task<bool> IsValidRedirect(Uri uri, CancellationToken cancellationToken)
    {
        if (uri == null)
        {
            return false;
        }
        
        if (workerConf.AllowInternalTargets == false)
        {
            await EnsureExternalTarget(uri, cancellationToken);
        }

        return true;
    }

    private async Task EnsureExternalTarget(Uri uri, CancellationToken cancellationToken)
    {
        if (uri.IsInternal() || (await ResolveHost(uri, cancellationToken)).Any(ipAddress => ipAddress.IsPrivate()))
        {
            throw new InternalTargetException($"Internal target detected at \"{uri.Host}\" when performing external scan.");
        }
    }

    private async Task<List<IPAddress>> ResolveHost(Uri uri, CancellationToken cancellationToken)
    {
        try
        {
            return (await Dns.GetHostAddressesAsync(uri.Host, cancellationToken))
                .Where(ipAddress => workerConf.IPv6Enabled || ipAddress.AddressFamily != AddressFamily.InterNetworkV6)
                .ToList();
        }
        catch (Exception)
        {
            throw new DnsResolutionException($"Could not resolve \"{uri.Host}\" to an IP address.");
        }
    }
}