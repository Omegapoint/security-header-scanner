namespace headers.security.Common.Domain.SecurityConcepts;

public class SecurityConceptResultInfo
{
    public string Message { get; init; }

    public SecurityConceptResultInfo(string message)
    {
        Message = message;
    }

    public static SecurityConceptResultInfo Create(string message) => new(message);
}