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
        private static KillPCScreen killPCScreen;
        private static RevivePCScreen revivePCScreen;
        private static KillScreen killScreen;
        
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
            screen.ShowKillScreen += ShowKillScreen;
            screen.ShowRevivePCScreen += ShowRevivePCScreen;
            screen.Exit += ExitApplication;
            screen.Show();
        }

        private static void ShowKillScreen(object? sender, ShowKillScreenEventArgs e)
        {
            killScreen = new KillScreen(combat, CombatantType.Monster);
            killScreen.Cancel += screen.KillCancel;
            killScreen.KillCombatant += (sender, args) =>
            {
                combat.KillCombatant(args.Combatant);
                screen.Show();
            };
            killScreen.Show();
        }
        
        private static void ShowRevivePCScreen(object? sender, EventArgs e)
        {
            revivePCScreen = new RevivePCScreen(combat);
            revivePCScreen.Cancel += screen.KillCancel;
            revivePCScreen.RevivePc += (sender, args) =>
            {
                combat.RevivePC(args.Combatant);
                screen.Show();
            };
            revivePCScreen.Show();
        }
    }
}



