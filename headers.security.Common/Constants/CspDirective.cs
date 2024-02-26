// ReSharper disable MemberCanBePrivate.Global
namespace headers.security.Common.Constants;

public static class CspDirective
{
    public const string DefaultSrc = "default-src";
    
    // Not covered by default-src
    public const string FrameAncestors = "frame-ancestors";
    public const string Referrer = "referrer";
    
    // Covered by default-src
    public const string ChildSrc = "child-src";
    public const string ConnectSrc = "connect-src";
    public const string FontSrc = "font-src";
    public const string FrameSrc = "frame-src";
    public const string ImgSrc = "img-src";
    public const string ManifestSrc = "manifest-src";
    public const string MediaSrc = "media-src";
    public const string ObjectSrc = "object-src";
    public const string PrefetchSrc = "prefetch-src";
    public const string ScriptSrc = "script-src";
    public const string ScriptSrcElem = "script-src-elem";
    public const string ScriptSrcAttr = "script-src-attr";
    public const string StyleSrc = "style-src";
    public const string StyleSrcElem = "style-src-elem";
    public const string StyleSrcAttr = "style-src-attr";
    public const string WorkerSrc = "worker-src";

    /// <summary>
    /// Documented at https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/default-src
    /// Code is up to date as long as page contains: "This page was last modified on Apr 10, 2023 by MDN contributors."
    /// </summary>
    private static readonly string[] CoveredByDefaultSrc = [
        ChildSrc,
        ConnectSrc,
        FontSrc,
        FrameSrc,
        ImgSrc,
        ManifestSrc,
        MediaSrc,
        ObjectSrc,
        PrefetchSrc,
        ScriptSrc,
        ScriptSrcElem,
        ScriptSrcAttr,
        StyleSrc,
        StyleSrcElem,
        StyleSrcAttr,
        WorkerSrc
    ];

    public static bool FallsBackToDefaultSrc(string directive) => CoveredByDefaultSrc.Contains(directive, StringComparer.OrdinalIgnoreCase);
}