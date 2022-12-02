using System;

namespace Initiative.Classes.Screens;

public class ScreenBase
{
    public static ConsoleColor SelectedBackground => ConsoleColor.DarkGreen;
    public static ConsoleColor SelectedForeground => ConsoleColor.Black;
    public static ConsoleColor DefaultBackground { get; set; }
    public static ConsoleColor DefaultForeground { get; set; }
    public static ConsoleColor ErrorBackground => ConsoleColor.Black;
    public static ConsoleColor ErrorForeground => ConsoleColor.Red;
    public static ConsoleColor DeadCombatantForegroundColor => ConsoleColor.DarkGray;
    public static ConsoleColor DeadCombatantBackgroundColor => DefaultBackground;

    public ScreenBase()
    {
        DefaultBackground = Console.BackgroundColor;
        DefaultForeground = Console.ForegroundColor;
    }
    
    public static void Reset()
    {
        Console.Clear();
        Console.SetCursorPosition(0,0);
        Console.ForegroundColor = DefaultForeground;
        Console.BackgroundColor = DefaultBackground;
    }
}