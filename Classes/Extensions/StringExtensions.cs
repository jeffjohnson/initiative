using System.Text;

namespace Initiative.Classes.Extensions;

/// <summary>
/// Extension methods for the String type.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Converts the string to a UTF8 encoded string.
    /// </summary>
    /// <returns>The UTF8 encoded string.</returns>
    public static string ToUTF8(this string str)
    {
        var bytes = Encoding.Default.GetBytes(str);
        return Encoding.UTF8.GetString(bytes);
    }

    /// <summary>
    /// Takes text and converts it to the same text with the Unicode combining long stroke overlay character.
    /// </summary>
    /// <returns>The strikethrough text.</returns>
    public static string ToStrikeThrough(this string str)
    {
        var result = "";
        foreach (var c in str)
            result += $"{c}\u0336";

        return result;
    }

    /// <summary>
    /// Quick method to check if string is blank or null.
    /// </summary>
    /// <param name="str">This string</param>
    /// <returns>True or False, depending on if blank or not.</returns>
    public static bool HasValue(this string str)
    {
        return !string.IsNullOrWhiteSpace(str);
    }
}