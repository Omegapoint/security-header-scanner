using headers.security.Common.Domain;
using headers.security.Scanner.Extensions;

namespace headers.security.Scanner.Configuration;

public class CrawlerConfiguration
{
    public TargetKind TargetKind { private get; set; }
    
    public bool FollowRedirects { get; set; }
    
    public CancellationToken CancellationToken { get; set; }

    public TargetKind GetTargetKind(HttpResponseMessage message) =>
        TargetKind == TargetKind.Detect
            ? message.DetectTargetKind()
            : TargetKind;
}