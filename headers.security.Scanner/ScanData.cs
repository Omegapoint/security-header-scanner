using headers.security.Common.Domain;

namespace headers.security.Scanner;

public class ScanData(
    HttpResponseMessage message,
    Uri target,
    CrawlerConfiguration crawlerConfiguration,
    RawHeaders rawHeaders,
    RawHeaders rawHttpEquivMetas)
{
    public readonly Uri Target = target;
    public readonly CrawlerConfiguration CrawlerConfiguration = crawlerConfiguration;
    public readonly HttpResponseMessage Message = message;
    public readonly RawHeaders RawHeaders = rawHeaders;
    public readonly RawHeaders RawHttpEquivMetas = rawHttpEquivMetas;

    public TargetKind TargetType => CrawlerConfiguration.GetTargetKind(Message);
}