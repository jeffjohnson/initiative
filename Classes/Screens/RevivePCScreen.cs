using ANSIConsole;
using Initiative.Classes.Screens.EventArgs;

namespace Initiative.Classes.Screens;

public class RevivePCScreen : ScreenBase
{
    private Combat Combat { get; set; }
    private int FirstPcId => Combat.PCs.First(x => x.IsDead).Id;
    private int SelectedPcId { get; set; }
    private int LastPcId => Combat.PCs.Last(x => x.IsDead).Id;
    public RevivePCScreen(Combat combat)
    {
        Combat = combat;
        SelectedPcId = FirstPcId;
    }

    public void Show()
    {
        Reset();
            
        Console.WriteLine("╭───────────╮");
        Console.WriteLine("│ Revive PC ╰────────────────────────────╮");
        Console.WriteLine("├────────────────────────────────────────┤");
        
        Redraw();
        
        var action = new ConsoleKeyInfo();
        while (action.Key != ConsoleKey.Q)
        {
            action = Console.ReadKey(true);
            switch (action.Key)
            {
                case ConsoleKey.UpArrow:
                    PreviousPC();
                    break;
                    
                case ConsoleKey.DownArrow:
                    NextPC();
                    break;
                   
                case ConsoleKey.K:
                case ConsoleKey.Enter:
                    RevivePc?.Invoke(this, new RevivePcEventArgs()
                    {
                        Combatant = Combat.PCs.ToList().First(x => x.Id == SelectedPcId)
                    });
                    break;
                
                case ConsoleKey.C:
                case ConsoleKey.Escape:
                    Cancel?.Invoke(this, System.EventArgs.Empty);
                    break;
            }
        }
    }

    private void Redraw()
    {
        Console.SetCursorPosition(0, 3);
        var deadPCs = Combat.PCs.Where(x => x.IsDead).ToList();

        foreach (var deadPC in deadPCs)
        {
            var pos = Console.GetCursorPosition();
            Console.ForegroundColor = DefaultForeground;
            Console.BackgroundColor = DefaultBackground;

            Console.Write("│                                        │");
            
            // change background current PC is selected
            if (deadPC.Id == SelectedPcId)
            {
                Console.ForegroundColor = SelectedForeground;
                Console.BackgroundColor = SelectedBackground;
                pos = Console.GetCursorPosition();
                Console.SetCursorPosition(2, pos.Top);
                Console.Write("                                     ");
            }
            
            Console.SetCursorPosition(2, pos.Top);

            Console.Write($"{deadPC.Name}");
            Console.Write(Environment.NewLine);
        }

        Console.ForegroundColor = DefaultForeground;
        Console.BackgroundColor = DefaultBackground;
        Console.WriteLine("╰┯──────────────────┯───────────────────┯╯");
        Console.WriteLine(" │ `#c81e64|↓´ next character │ `#c81e64|r´evive selected `#c81e64|↲´ │".FormatANSI());
        Console.WriteLine(" │ `#c81e64|↑´ prev character │ `#c81e64|c´ancel `#c81e64|ESC´        │".FormatANSI());
        Console.WriteLine(" ╰──────────────────┷───────────────────╯");
    }

    private void PreviousPC()
    {
        if (SelectedPcId == FirstPcId)
            return;
        
        var deadPCs = Combat.PCs
            .Where(x => x.IsDead).ToList()
            .Select(x => x.Id).ToArray();
        var ix = Array.IndexOf(deadPCs, SelectedPcId);
        SelectedPcId = deadPCs[ix - 1];
        
        Redraw();
    }

    private void NextPC()
    {
        if (SelectedPcId == LastPcId)
            return;

        var deadPCs = Combat.PCs
            .Where(x => x.IsDead).ToList()
            .Select(x => x.Id).ToArray();
        var ix = Array.IndexOf(deadPCs, SelectedPcId);
        SelectedPcId = deadPCs[ix + 1];
        Redraw();
    }
    
    public event EventHandler<EventArgs.RevivePcEventArgs>? RevivePc;
    public event EventHandler? Cancel;
}