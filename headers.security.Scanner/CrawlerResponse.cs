using System.Net;
using headers.security.Common.Domain;

namespace headers.security.Scanner;

public class CrawlerResponse
{
    // ReSharper disable once InconsistentNaming
    public IPAddress IP { get; init; }
    public Uri FinalUri { get; set; }
    public HttpResponseMessage HttpMessage { get; set; }
    public ScanError ScanError { get; set; }
    public DateTime FetchedAt { get; set; }
    public bool IsFailure => ScanError != null;
}