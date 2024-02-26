using headers.security.Common.Domain;

namespace headers.security.Common.Extensions;

public static class SecurityGradeExtensions
{
    public static SecurityGrade Lowered(this SecurityGrade grade, int diff)
        => (SecurityGrade) Math.Max((int) SecurityGrade.F, (int) grade - diff);

    public static SecurityGrade Lowered(this SecurityGrade grade, bool shouldLower)
        => grade.Lowered(shouldLower ? 1 : 0);
}

public static class HttpResponseMessageExtensions
{
    public static bool IsRedirectStatusCode(this HttpResponseMessage message)
    {
        return (int) message.StatusCode is >= 300 and < 400;
    }
}