using headers.security.Common.Domain;

namespace headers.security.Common.Extensions;

public static class SecurityImpactExtensions
{
    public static SecurityImpact Lowered(this SecurityImpact impact, int diff)
        => (SecurityImpact) Math.Max((int) SecurityImpact.Critical, Math.Max((int) impact - diff, 0));

    public static SecurityImpact Lowered(this SecurityImpact impact, bool shouldLower)
        => impact.Lowered(shouldLower ? 1 : 0);
}