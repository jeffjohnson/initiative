using Spectre.Console;

namespace Initiative.Classes;

public static class Screen
{
    public static void Reset()
    {
        Console.Clear();
        Console.SetCursorPosition(0,0);
    }

    public static void DrawCombatTable(Combat combat)
    {
        Reset();

        var table = new Table();
        table.Title = new TableTitle("Initiative Tracker");
        
        AnsiConsole.Write(table);
    }
}