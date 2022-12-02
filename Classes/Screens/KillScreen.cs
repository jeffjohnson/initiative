using ANSIConsole;
using Initiative.Classes.Screens.EventArgs;

namespace Initiative.Classes.Screens;

public class KillScreen : ScreenBase
{
    bool disposed = false;
    private readonly CombatantType type;
    private Combat Combat { get; set; }
    private List<Combatant> Combatants { get; set; }
    private int FirstId => Combatants.First(x => x.IsDead != true).Id;
    private int SelectedId { get; set; }
    private int LastId => Combatants.Last(x => x.IsDead != true).Id;
    public KillScreen(Combat combat, CombatantType combatantType)
    {
        type = combatantType;
        Combat = combat;

        switch (type)
        {
            case CombatantType.Monster:
                Combatants = Combat.Monsters;
                break;
            
            case CombatantType.PC:
                Combatants = Combat.PCs;
                break;
        }
        
        // do this after setting the Combatants property
        SelectedId = FirstId;
    }

    public void Show()
    {
        Reset();

        var windowHeader = "Unknown";
        switch (type)
        {
            case CombatantType.Monster:
                windowHeader = "Kill Monster";
                break;

            case CombatantType.PC:
                windowHeader = "Kill PC";
                break;
        }

        Console.WriteLine($"╭─{"─".PadLeft(windowHeader.Length, '─')}─╮");
        Console.WriteLine($"│ {windowHeader} ╰{"─".PadLeft(35 - windowHeader.Length, '─')}╮");
        Console.WriteLine($"├─{"─".PadLeft(37, '─')}┤");

        DrawShell();
        DrawCombatants();
        Listen();
    }

    private void DrawShell()
    {
        Console.SetCursorPosition(0, 3);
        Console.ForegroundColor = DefaultForeground;
        Console.BackgroundColor = DefaultBackground;
        
        foreach (var combatant in Combatants.Where(x => x.IsDead != true))
        {
            var pos = Console.GetCursorPosition();
            Console.WriteLine("│                                      │");
        }

        Console.ForegroundColor = DefaultForeground;
        Console.BackgroundColor = DefaultBackground;
        Console.WriteLine("╰┯──────────────────┯─────────────────┯╯");
        Console.WriteLine(" │ `#c81e64|↓´ next combatant │ `#c81e64|k´ill selected `#c81e64|↲´ │".FormatANSI());
        Console.WriteLine(" │ `#c81e64|↑´ prev combatant │ `#c81e64|c´ancel `#c81e64|ESC´      │".FormatANSI());
        Console.WriteLine(" ╰──────────────────┷─────────────────╯");
    }

    private void DrawCombatants()
    {
        Console.SetCursorPosition(0, 3);

        foreach (var combatant in Combatants.Where(x => x.IsDead != true))
        {
            var pos = Console.GetCursorPosition();
            Console.ForegroundColor = DefaultForeground;
            Console.BackgroundColor = DefaultBackground;

            // change background current combatant is selected
            if (combatant.Id == SelectedId)
            {
                Console.ForegroundColor = SelectedForeground;
                Console.BackgroundColor = SelectedBackground;
            }
            
            Console.SetCursorPosition(1, pos.Top);
            Console.Write("                                      ");
            Console.SetCursorPosition(2, pos.Top);
            Console.Write($"{combatant.Name}");
            Console.Write(Environment.NewLine);
        }
    }

    private void Listen()
    {
        var action = new ConsoleKeyInfo();
        while (action.Key != ConsoleKey.F24)
        {
            action = Console.ReadKey(true);
            switch (action.Key)
            {
                case ConsoleKey.UpArrow:
                    SelectPrevious();
                    break;
                    
                case ConsoleKey.DownArrow:
                    SelectNext();
                    break;
                   
                case ConsoleKey.K:
                case ConsoleKey.Enter:
                    KillCombatant?.Invoke(this,
                        new SpecifyCombatantEventArgs(Combatants.First(x => x.Id == SelectedId)));

                    break;
                
                case ConsoleKey.C:
                case ConsoleKey.Escape:
                    Cancel?.Invoke(this, System.EventArgs.Empty);
                    break;
            }
        }
    }

    private void SelectPrevious()
    {
        if (SelectedId == FirstId)
            return;
        
        var aliveCombatants = Combatants
            .Where(x => x.IsDead != true).ToList()
            .Select(x => x.Id).ToArray();
        var ix = Array.IndexOf(aliveCombatants, SelectedId);
        SelectedId = aliveCombatants[ix - 1];
        
        DrawCombatants();
    }

    private void SelectNext()
    {
        if (SelectedId == LastId)
            return;

        var aliveCombatants = Combatants
            .Where(x => x.IsDead != true).ToList()
            .Select(x => x.Id).ToArray();
        var ix = Array.IndexOf(aliveCombatants, SelectedId);
        SelectedId = aliveCombatants[ix + 1];
        
        DrawCombatants();
    }
    
    public event EventHandler<SpecifyCombatantEventArgs>? KillCombatant;
    public event EventHandler? Cancel;
}