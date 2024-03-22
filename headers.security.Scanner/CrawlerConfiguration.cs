using headers.security.Common.Domain;
using headers.security.Scanner.Extensions;

namespace headers.security.Scanner;

// TODO: add configuration support for things like authorization (should perform test both with and without)
// TODO: should also support setting method verb to use, per line if using file
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