using System;
using System.IO;
using Initiative.Classes.Screens;

namespace Initiative.Classes;

public static class ConsoleHelper
{
    public static string ReadInput()
    {
        var value = new StringWriter();
        ConsoleKeyInfo key;
        do
        {
            key = Console.ReadKey(true);
            
            

            if (key.Modifiers == ConsoleModifiers.Control)
                return $"ctrl+{key.Key}".ToLower();

            if (key.Key != ConsoleKey.Enter)
            {
                Console.Write(key.KeyChar.ToString());
                value.Write(key.KeyChar.ToString());
            }
            else
            {
                var x = key.KeyChar;
            }

        } while (key.Key != ConsoleKey.Enter && key.Modifiers != ConsoleModifiers.Control);


        return value.ToString();
    }

    public static void ClearColumns(int fromColumn, int toColumn, int height)
    {
        Console.ForegroundColor = ScreenBase.DefaultForeground;
        Console.BackgroundColor = ScreenBase.DefaultBackground;
        for (var i = 0; i < height; i++)
        {
            Console.SetCursorPosition(fromColumn, i);
            Console.Write(" ".PadLeft(toColumn - fromColumn, ' '));
        }
    }
}