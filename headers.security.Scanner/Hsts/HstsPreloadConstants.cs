namespace headers.security.Scanner.Hsts;

public static class HstsPreloadConstants
{
    private const string Upstream =
        "https://chromium.googlesource.com/chromium/src/+/main/net/http/transport_security_state_static.json?format=TEXT";

    public static readonly Uri UpstreamUri = new(Upstream);
}