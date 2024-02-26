using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace headers.security.Frontend;

[DataContract]
public class ScanRequestContract : IValidatableObject
{
    [Required]
    public Uri Target { get; set; }
    
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