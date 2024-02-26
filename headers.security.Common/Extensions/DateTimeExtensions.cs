namespace headers.security.Common.Extensions;

public static class DateTimeExtensions
{
    /// <summary>
    /// Returns the given DateTime as a string in the format "yyyy.MM.dd"
    /// </summary>
    public static string SimpleDateString(this DateTime dateTime)
    {
        return dateTime.ToString("yyyy.MM.dd");
    }
}