namespace headers.security.Common;

public class StringDifferentiator
{
    public string Value { get; }

    private StringDifferentiator(string str) => Value = str;

    public static implicit operator StringDifferentiator(string str) => new(str);

    public static implicit operator StringDifferentiator(FormattableString formattable) => new(formattable.ToString());
}