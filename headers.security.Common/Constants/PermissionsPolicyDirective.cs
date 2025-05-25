// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
// ReSharper disable MemberCanBePrivate.Global
namespace headers.security.Common.Constants;

public static class PermissionsPolicyDirective
{
    public const string Bluetooth      = "bluetooth";
    public const string Camera         = "camera";
    public const string DisplayCapture = "display-capture";
    public const string Geolocation    = "geolocation";
    public const string HID            = "hid";
    public const string Microphone     = "microphone";
    public const string Payment        = "payment";
    public const string Serial         = "serial";
    public const string USB            = "usb";

    public static readonly List<string> SensitiveDirectives = [
        Bluetooth,
        Camera,
        DisplayCapture,
        Geolocation,
        HID,
        Microphone,
        Payment,
        Serial,
        USB,
    ];
}