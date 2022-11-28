using ANSIConsole;
using Initiative.Classes.Screens.EventArgs;

namespace Initiative.Classes.Screens;

public class KillMonsterScreen : ScreenBase
{
    private Combat Combat { get; set; }
    private int FirstMonsterId => Combat.Monsters().First().Id;
    private int SelectedMonsterId { get; set; }
    private int LastMonsterId => Combat.Monsters().Last().Id;
    public KillMonsterScreen(Combat combat)
    {
        Combat = combat;
        SelectedMonsterId = FirstMonsterId;
    }

    public void Show()
    {
        Reset();
            
        Console.WriteLine("╭──────────────╮");
        Console.WriteLine("│ Kill Monster ╰─────────────────────╮");
        Console.WriteLine("├────────────────────────────────────┤");
        
        Redraw();
        
        var action = new ConsoleKeyInfo();
        while (action.Key != ConsoleKey.Q)
        {
            action = Console.ReadKey(true);
            switch (action.Key)
            {
                case ConsoleKey.UpArrow:
                    PreviousMonster();
                    break;
                    
                case ConsoleKey.DownArrow:
                    NextMonster();
                    break;
                   
                case ConsoleKey.Enter:
                    Console.Clear();
                    var args = new KillEventArgs();
                    args.Combatant = Combat.Monsters().ToList().First(x => x.Id == SelectedMonsterId);
                    KillMonster?.Invoke(this, args);
                    break;
                
                case ConsoleKey.C:
                    Console.Clear();
                    Cancel?.Invoke(this, System.EventArgs.Empty);
                    break;
            }
        }
    }

    private void Redraw()
    {
        Console.SetCursorPosition(0, 3);
        var monsters = Combat.Monsters().Where(x => x.IsDead != true).ToList();

        foreach (var monster in monsters)
        {
            var pos = Console.GetCursorPosition();
            Console.ForegroundColor = DefaultForeground;
            Console.BackgroundColor = DefaultBackground;

            Console.Write("│                                    │");
            
            // change background current monster is selected
            if (monster.Id == SelectedMonsterId)
            {
                Console.ForegroundColor = SelectedForeground;
                Console.BackgroundColor = SelectedBackground;
                pos = Console.GetCursorPosition();
                Console.SetCursorPosition(2, pos.Top);
                Console.Write("                                 ");
            }
            
            Console.SetCursorPosition(2, pos.Top);

            Console.Write($"{monster.Name}");
            Console.Write(Environment.NewLine);
        }

        Console.ForegroundColor = DefaultForeground;
        Console.BackgroundColor = DefaultBackground;
        Console.WriteLine("╰┯────────────────┯─────────────────┯╯");
        Console.WriteLine(" │ `Red|↓´ next monster │ kill selected `Red|↲´ │".FormatANSI());
        Console.WriteLine(" │ `Red|↑´ prev monster │ `Red|c´ancel          │".FormatANSI());
        Console.WriteLine(" ╰────────────────┷─────────────────╯");
    }

    private void PreviousMonster()
    {
        if (SelectedMonsterId == FirstMonsterId)
            return;
        
        var arr = Combat.Monsters().ToList().Select(x => x.Id).ToArray();
        var ix = Array.IndexOf(arr, SelectedMonsterId);
        SelectedMonsterId = arr[ix - 1];
        
        Redraw();
    }

    private void NextMonster()
    {
        if (SelectedMonsterId == LastMonsterId)
            return;

        var arr = Combat.Monsters().ToList().Select(x => x.Id).ToArray();
        var ix = Array.IndexOf(arr, SelectedMonsterId);
        SelectedMonsterId = arr[ix + 1];
        Redraw();
    }
    
    public event EventHandler<EventArgs.KillEventArgs>? KillMonster;
    public event EventHandler? Cancel;
}