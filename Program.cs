using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Encodings.Web;
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
        private static KillMonsterScreen killMonsterScreen;
        
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.TreatControlCAsInput = true;
            Console.CursorVisible = false;

            NewCombat(null, new NewCombatEventArgs()
            {
                PCs = pcs
            });
            
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
            screen.StartNewCombat += NewCombat;
            screen.ShowKillMonsterScreen += ShowKillMonsterScreen;
            screen.Exit += ExitApplication;
            screen.Show();
        }

        private static void ShowKillMonsterScreen(object? sender, EventArgs e)
        {
            killMonsterScreen = new KillMonsterScreen(combat);
            killMonsterScreen.Cancel += screen.KillCancel;
            killMonsterScreen.KillMonster += (sender, ea) =>
            {
                combat.KillCombatant(ea.Combatant);
                screen.Show();
            };
            killMonsterScreen.Show();
        }
    }
}



