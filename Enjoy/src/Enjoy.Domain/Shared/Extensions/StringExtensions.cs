using System.Text.RegularExpressions;

namespace Enjoy.Domain.Shared.Extensions;

public static class StringExtensions
{
    public static string[] SplitByWhitespace(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return [];
        }

        string pattern = @"\s+";
        var regex = new Regex(pattern);
        return regex.Split(input);
    }

    public static string Capitalize(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        if (input.Length == 1)
            return input.ToUpperInvariant();

        return char.ToUpperInvariant(input[0]) + input[1..].ToLowerInvariant();
    }

    public static string ReplaceWhitespace(this string input, string replacement)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        string pattern = @"\s+";
        var regex = new Regex(pattern);
        return regex.Replace(input, replacement);
    }

    public static string CapitalizeWords(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        var words = input.SplitByWhitespace();
        for (int i = 0; i < words.Length; i++)
        {
            words[i] = words[i].Capitalize();
        }

        return string.Join(" ", words);
    }

    public static string GetValueOrDefault(this string input, string defaultValue) => string.IsNullOrEmpty(input) ? defaultValue : input;

    public static string NormalizeWhitespace(this string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return Regex.Replace(input.Trim(), @"\s+", " ");
    }
}
