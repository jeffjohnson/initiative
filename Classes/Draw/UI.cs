using System;

namespace Initiative.Classes.Draw;

public static class UI
{
    public static readonly string HEADER01 =
        "╭─────────────╮                                                                 ";

    public static readonly string HEADER02 =
        "│ Initiative  ╰────────┬───┬───┬───┬───┬─╼ ROUND ╾─┬───┬────┬────┬────┬────┬───╮";

    public static void DrawCombat(Combat combat)
    {
        ResetScreen();
        Console.WriteLine(HEADER01);
        Console.WriteLine(HEADER02);


    }


    public static void ResetScreen()
    {
        Console.Clear();
        Console.SetCursorPosition(0,0);
    }
    public static Combatant? DrawAddCombatant()
    {
        ResetScreen();
        var offset = Console.GetCursorPosition();
        Console.WriteLine("╭───────────────╮");
        Console.WriteLine("│ Add Combatant ╰──────────────╮");
        Console.WriteLine("├──────────────────────────────┤");
        Console.WriteLine("│ Name:                        │"); //  8,3 pos start                                             
        Console.WriteLine("│ Player:                      │"); // 10,4 pos start
        Console.WriteLine("│ Initiative Bonus:            │"); // 20,5 pos start
        Console.WriteLine("╰┯─────────┯───────────┯───────╯");
        Console.WriteLine(" ╰╼ ⌃save ╾┴╼ ⌃cancel ╾╯");         // s = 4,7 -> c = 14,7
        Console.ForegroundColor = ConsoleColor.Red;
        Console.SetCursorPosition(offset.Left + 4,offset.Top + 7);
        Console.Write("⌃s");
        Console.SetCursorPosition(offset.Left + 14,offset.Top + 7);
        Console.Write("⌃c");
        Console.ForegroundColor = ConsoleColor.White;

        var combatant = new Combatant();
        var name = "";
        var player = "";
        var initiativeBonus = 0;
        
        start_read:
        // read name
        Console.SetCursorPosition(offset.Left + 8, offset.Top + 3);        
        name = ConsoleHelper.ReadInput();

        if (name == "ctrl+c")
            return null;

        if (name == "ctrl+s")
            return combatant;

        combatant.Name = name;
        
        // read player
        Console.SetCursorPosition(offset.Left + 10, offset.Top + 4);
        player = ConsoleHelper.ReadInput();

        if (player == "ctrl+c")
            return null;

        if (player == "ctrl+s")
            return combatant;

        combatant.Player = player;
        
        // read initiative bonus
        Console.SetCursorPosition(offset.Left + 20, offset.Top + 5);
        var init = "";
        init = ConsoleHelper.ReadInput();
        init = init.TrimStart('+');

        int.TryParse(init, out initiativeBonus);

        if (init == "ctrl+c")
            return null;

        if (init == "ctrl+s")
            return combatant;

        combatant.InitiativeBonus = initiativeBonus;
        
        goto start_read;
    }
}