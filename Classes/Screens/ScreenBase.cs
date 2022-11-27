using System;

namespace Initiative.Classes.Screens;

public class ScreenBase
{
    public static ConsoleColor SelectedBackground => ConsoleColor.DarkGreen;
    public static ConsoleColor SelectedForeground => ConsoleColor.Black;
    public static ConsoleColor DefaultBackground => ConsoleColor.Black;
    public static ConsoleColor DefaultForeground => ConsoleColor.White;
    public static ConsoleColor ErrorBackground => ConsoleColor.Red;
    public static ConsoleColor ErrorForeground => ConsoleColor.Black;
    public static ConsoleColor DeadCombatantForegroundColor => ConsoleColor.DarkGray;
    public static ConsoleColor DeadCombatantBackgroundColor => ConsoleColor.Black;
    
    public static void Reset()
    {
        Console.Clear();
        Console.SetCursorPosition(0,0);
    }
}