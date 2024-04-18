using headers.security.Scanner.SecurityConcepts.Csp;
using static headers.security.Common.Constants.CspDirective;
using static headers.security.Common.Constants.CspToken;

namespace headers.security.Api.Middlewares;

public static class SiteCspConfiguration
{
    public static string Policy =>
        new CspDirectiveCollection
        {
            [DefaultSrc] = [Self],
            [FontSrc] = [Self, Data],
            [StyleSrc] = [Self, UnsafeInline],
            [FormAction] = [Self],
            [FrameAncestors] = [None]
        }.ToString();
}