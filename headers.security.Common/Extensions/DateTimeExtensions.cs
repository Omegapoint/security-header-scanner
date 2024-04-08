namespace headers.security.Common.Extensions;

public static class DateTimeExtensions
{
    /// <summary>
    /// Returns the given DateTime as a string in the format "yyyy.MM.dd"
    /// </summary>
    public static string VersionDateString(this DateTime dateTime)
    {
        var secondsSinceMidnight = (int) dateTime.Subtract(dateTime.Date).TotalSeconds;
        return dateTime.ToString("yyyy.MM.dd-") + secondsSinceMidnight;
    }
}