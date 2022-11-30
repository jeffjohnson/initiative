using ANSIConsole;
using Initiative.Classes.Screens.EventArgs;

namespace Initiative.Classes.Screens;

public class KillPCScreen : ScreenBase
{
    private Combat Combat { get; set; }
    private int FirstPcId => Combat.PCs.First(x => x.IsDead != true).Id;
    private int SelectedPcId { get; set; }
    private int LastPcId => Combat.PCs.Last(x => x.IsDead != true).Id;
    public KillPCScreen(Combat combat)
    {
        Combat = combat;
        SelectedPcId = FirstPcId;
    }

    public void Show()
    {
        Reset();
            
        Console.WriteLine("╭─────────╮");
        Console.WriteLine("│ Kill PC ╰────────────────────────────╮");
        Console.WriteLine("├──────────────────────────────────────┤");
        
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
                    KillPC?.Invoke(this, new KillEventArgs()
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
        var pcs = Combat.PCs.Where(x => x.IsDead != true).ToList();

        foreach (var pc in pcs)
        {
            var pos = Console.GetCursorPosition();
            Console.ForegroundColor = DefaultForeground;
            Console.BackgroundColor = DefaultBackground;

            Console.Write("│                                      │");
            
            // change background current PC is selected
            if (pc.Id == SelectedPcId)
            {
                Console.ForegroundColor = SelectedForeground;
                Console.BackgroundColor = SelectedBackground;
                pos = Console.GetCursorPosition();
                Console.SetCursorPosition(2, pos.Top);
                Console.Write("                                   ");
            }
            
            Console.SetCursorPosition(2, pos.Top);

            Console.Write($"{pc.Name}");
            Console.Write(Environment.NewLine);
        }

        Console.ForegroundColor = DefaultForeground;
        Console.BackgroundColor = DefaultBackground;
        Console.WriteLine("╰┯──────────────────┯─────────────────┯╯");
        Console.WriteLine(" │ `Red|↓´ next character │ `Red|k´ill selected `Red|↲´ │".FormatANSI());
        Console.WriteLine(" │ `Red|↑´ prev character │ `Red|c´ancel `Red|ESC´      │".FormatANSI());
        Console.WriteLine(" ╰──────────────────┷─────────────────╯");
    }

    private void PreviousPC()
    {
        if (SelectedPcId == FirstPcId)
            return;
        
        var alivePCs = Combat.PCs
            .Where(x => !x.IsDead).ToList()
            .Select(x => x.Id).ToArray();
        var ix = Array.IndexOf(alivePCs, SelectedPcId);
        SelectedPcId = alivePCs[ix - 1];
        
        Redraw();
    }

    private void NextPC()
    {
        if (SelectedPcId == LastPcId)
            return;

        var alivePCs = Combat.PCs
            .Where(x => !x.IsDead).ToList()
            .Select(x => x.Id).ToArray();
        var ix = Array.IndexOf(alivePCs, SelectedPcId);
        SelectedPcId = alivePCs[ix + 1];
        Redraw();
    }
    
    public event EventHandler<EventArgs.KillEventArgs>? KillPC;
    public event EventHandler? Cancel;
}