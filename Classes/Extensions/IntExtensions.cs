namespace Initiative.Classes.Extensions;

/// <summary>
/// Extension methods for the Int32 type.
/// </summary>
public static class IntExtensions
{
    /// <summary>
    /// Converts a number to a minimum of four digit string with leading spaces.
    /// </summary>
    /// <returns>A string representation of the integer padded with leading spaces.</returns>
    public static string FourPadded(this int i)
    {
        return i.ToString().PadLeft(4, ' ');
    }
}