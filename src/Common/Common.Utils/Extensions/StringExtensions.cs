namespace Common.Utils.Extensions;

public static class StringExtensions
{
    public static string RemoveAllApostrophes(this string text)
    {
        return text.Replace("'", "");
    }

    public static string RemoveDotIfIsAtTheEnd(this string text)
    {
        return text[^1].Equals('.') ? text[..^1] : text;
    }
}
