using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Initiative.Classes;
using Initiative.Classes.Screens;
using Initiative.Classes.Screens.EventArgs;

namespace Initiative
{
    class Program
    {
        private static readonly string pcFilePath = "pcs.json";
        static List<Combatant> pcs = JsonSerializer.Deserialize<List<Combatant>>(File.ReadAllText(pcFilePath));
        
        private static Combat combat;
        private static InitiativeTrackerScreen screen;
        
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.TreatControlCAsInput = true;
            Console.CursorVisible = false;

            combat = new Combat(pcs);
            combat.StartCombat();
            
            screen = new InitiativeTrackerScreen(combat);
            screen.Exit += ExitApplication;
            screen.Show();
            
            Process.GetCurrentProcess().WaitForExit();
        }

        private static void ExitApplication(object? sender, ExitEventArgs e)
        {
            //File.WriteAllText(pcFilePath, JsonSerializer.Serialize(e.PCs));
            Environment.Exit(0);
        }

        private static void NewCombat(object? sender, NewCombatEventArgs e)
        {
            combat = new Combat(e.PCs);
            combat.StartCombat();
            
            screen = new InitiativeTrackerScreen(combat);
            screen.Exit += ExitApplication;
            screen.Show();
        }
    }
}



