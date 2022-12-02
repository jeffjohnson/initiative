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
        private static RevivePCScreen revivePCScreen;
        private static KillScreen killScreen;
        private static AddMonsterScreen addMonsterScreen;
        
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.TreatControlCAsInput = true;
            Console.CursorVisible = false;

            NewCombat(null, new NewCombatEventArgs()
            {
                PCs = pcs
            });
            
            // Console.WriteLine("               0123456789012345678901234567890123456789012345678901234567890123456789");
            // Console.Write("Input Testing: ");
            // var txtTest = new TextInput(Console.CursorLeft, Console.CursorTop);
            // txtTest.TextCursorPositionChanged += PrintExcessData;
            // txtTest.Focus();
            
            Process.GetCurrentProcess().WaitForExit();
        }

        private static void PrintExcessData(object? sender, TextCursorPositionChangedEventArgs e)
        {
            Console.SetCursorPosition(0, e.CursorTop + 5);
            Console.WriteLine("                                                                          ");
            Console.SetCursorPosition(0, e.CursorTop + 5);
            Console.Write($"Text Cursor Position: {e.TextCursorPosition}, Value: [{new string(e.Value)}]");
            Console.SetCursorPosition(e.CursorLeft, e.CursorTop);
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
            screen.ShowAddMonsterScreen += ShowAddMonsterScreen;
            screen.Exit += ExitApplication;
            screen.Show();
        }

        private static void ShowKillScreen(object? sender, ShowKillScreenEventArgs e)
        {
            killScreen = new KillScreen(combat, e.type);
            killScreen.Cancel += (sender, args) =>
            {
                screen.Show();
            };
            
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
            revivePCScreen.Cancel += (sender, args) =>
            {
                screen.Show();
            };
            
            revivePCScreen.RevivePc += (sender, args) =>
            {
                combat.RevivePC(args.Combatant);
                screen.Show();
            };
            
            revivePCScreen.Show();
        }

        private static void ShowAddMonsterScreen(object? sender, EventArgs e)
        {
            addMonsterScreen = new AddMonsterScreen(combat);
            addMonsterScreen.Cancel += (sender, args) =>
            {
                screen.Show();
            };
            
            addMonsterScreen.SaveMonster += (sender, args) =>
            {
                combat.AddCombatant(args.Combatant);
                screen.Show();
            };
            
            addMonsterScreen.Show();
        }
    }
}



