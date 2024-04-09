using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using headers.security.Common.Domain;

namespace headers.security.Api.Contracts;

[DataContract]
public class ScanRequestContract : IValidatableObject
{
    [Required]
    public Uri Target { get; set; }

    public TargetKind Kind { get; set; } = TargetKind.Detect;
    
    public bool FollowRedirects { get; set; }

    private static readonly List<string> ValidSchemes = [Uri.UriSchemeHttp, Uri.UriSchemeHttps];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!ValidSchemes.Contains(Target.Scheme))
        {
            yield return new ValidationResult($"Uri scheme must be one of: {string.Join(", ", ValidSchemes)}");
        }
    }
}