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
        
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.TreatControlCAsInput = true;
            Console.CursorVisible = false;

            var combat = new Combat(pcs);
            combat.StartCombat();
            
            var screen = new InitiativeTrackerScreen(combat);
            screen.Exit += ExitApplication;
            screen.Show();
            
            Process.GetCurrentProcess().WaitForExit();
        }

        private static void ExitApplication(object? sender, ExitEventArgs e)
        {
            //File.WriteAllText(pcFilePath, JsonSerializer.Serialize(e.PCs));
            Environment.Exit(0);
        }
    }
}



