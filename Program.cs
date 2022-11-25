using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Initiative.Classes;
using Spectre.Console;


namespace Initiative
{
    class Program
    {
        private static readonly string pcFilePath = "pcs.json";
        static List<Combatant> pcs = JsonSerializer.Deserialize<List<Combatant>>(File.ReadAllText(pcFilePath));
        
        static void Main(string[] args)
        {
            
            AnsiConsole.Markup("[dim red]Hello[/] World!");
            var table = new Table();

// Add some columns
            table.AddColumn("Foo");
            table.AddColumn(new TableColumn("Bar").Centered());

// Add some rows
            table.AddRow("Baz", "[green]Qux[/]");
            table.AddRow(new Markup("[blue]Corgi[/]"), new Panel("Waldo"));

// Render the table to the console
            AnsiConsole.Write(table);
        //     Console.OutputEncoding = System.Text.Encoding.UTF8;
        //     Console.TreatControlCAsInput = true;
        //     Console.CursorVisible = true;
        //     
        //     var action = "";
        //     var combat = new Combat(pcs);
        //
        //     while (action != "x")
        //     {
        //         action = Console.ReadLine();
        //
        //         switch (action.ToLower())
        //         {
        //             case "a":
        //                 var combatant = GetCombatantDetails();
        //                 if (combatant != null)
        //                 {
        //                     combat.AddCombatant(combatant);
        //                     UI.DrawCombat(combat);
        //                 }
        //                 
        //                 break;
        //             case "r":
        //                 combat.NextRound();
        //                 UI.DrawCombat(combat);
        //                 break;
        //             case "q":
        //                 File.WriteAllText(pcFilePath, JsonSerializer.Serialize(pcs));
        //                 Environment.Exit(0);
        //                 break;
        //             default:
        //                 UI.DrawCombat(combat);
        //                 break;
        //         }
        //     }
        // }
        //
        // private static Combatant? GetCombatantDetails()
        // {
        //     var result = UI.DrawAddCombatant();
        //     UI.ResetScreen();
        //     
        //     if (result == null)
        //         Console.WriteLine("You canceled!");
        //     else
        //     {
        //         Console.WriteLine($"Name: {result.Name}");
        //         Console.WriteLine($"Player: {result.Player}");
        //         Console.WriteLine($"Initiative Bonus: {result.InitiativeBonus}");
        //     }
        //
        //     return result;
        }
    }
}



