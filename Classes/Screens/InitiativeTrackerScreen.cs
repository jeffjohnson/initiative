using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ANSIConsole;
using Initiative.Classes.Extensions;
using Initiative.Classes.Screens.EventArgs;

namespace Initiative.Classes.Screens;

public class InitiativeTrackerScreen : ScreenBase
{
    private const int MIN_COLUMNS = 10;
    private Combat Combat { get; set; }
    private int FirstCombatantId => Combat.CurrentInitiativeOrder.First().CombatantId;
    private int SelectedCombatantId { get; set; }
    private int LastCombatantId => Combat.CurrentInitiativeOrder.Last().CombatantId;
    private int actionLineTop = 4;

    private SortedDictionary<int, List<CombatantInitiative>> rounds { get; set; }

    public InitiativeTrackerScreen(Combat combat) : base()
    {
        Combat = combat;
        Overflow += combat.HandleOverflow;
        combat.DataChanged += CombatDataChanged;
    }

    public void Show()
    {
        Reset();

        SelectedCombatantId = Combat.CurrentRound.Initiatives.First().CombatantId;
        
        DrawHeader();
        DrawShell();
        DrawCombatants();
        //Redraw();
        
        var action = new ConsoleKeyInfo();
        while (action.Key != ConsoleKey.Q)
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
                    var aliveMonsters = Combat.Monsters.Where(x => !x.IsDead);
                    if (!aliveMonsters.Any())
                    {
                        DrawError(Error.NoMonstersToKill);
                    }
                    else
                    {
                        var killMonsterScreen = new KillMonsterScreen(Combat);
                        killMonsterScreen.Cancel += KillCancel;
                        killMonsterScreen.KillMonster += KillCombatant;
                        killMonsterScreen.Show();
                    }

                    break;
                
                case ConsoleKey.X:
                    Combat = new Combat(Combat.PCs);
                    Show();
                    break;
                   
                case ConsoleKey.Q:
                    Console.Clear();
                    var args = new ExitEventArgs()
                    {
                        PCs = Combat.PCs
                    };

                    Exit?.Invoke(this, args);
                    break;
            }
        }
    }

    private void DrawHeader()
    {
        Console.SetCursorPosition(0, 0);
        Console.WriteLine("╭────────────╮");
        Console.WriteLine("│ Initiative ╰────────────");
        Console.WriteLine("│  Tracker                ");
        Console.WriteLine("├─────────────────────────");
        
        var columnCount = Combat.Rounds.Count < MIN_COLUMNS ? MIN_COLUMNS : Combat.Rounds.Count;
        
        for (var i = 1; i <= columnCount; i++)
        {
            Console.SetCursorPosition(22 + i * 4, 1);
            Console.Write("───┬");
            
            Console.SetCursorPosition(22 + i * 4, 2);
            Console.Write(i.FourPadded());
            
            Console.SetCursorPosition(22 + i * 4, 3);
            Console.Write("────");
        }
        
        var pos = Console.GetCursorPosition();
        Console.SetCursorPosition(23 + (columnCount * 4) / 2, 1);
        
        if (columnCount % 2 == 0)
            Console.Write("╼ ROUND ╾");
        else
            Console.Write("┮ ROUND ┭");
        
        Console.SetCursorPosition(pos.Left, 1);
        Console.Write($"──╮{Environment.NewLine}");
        
        Console.SetCursorPosition(pos.Left, 2);
        Console.Write($"  │{Environment.NewLine}");
        
        Console.SetCursorPosition(pos.Left, 3);
        Console.Write($"──┤{Environment.NewLine}");
    }

    private void DrawShell()
    {
        ClearErrors();
        Console.SetCursorPosition(0, 4);
        
        var columnCount = Combat.Rounds.Count < MIN_COLUMNS ? MIN_COLUMNS : Combat.Rounds.Count;
        
        var left = 28 + (columnCount * 4);
        
        Combat.CurrentRound.Initiatives.ForEach((x) => { 
            Console.Write("│");
            Console.SetCursorPosition(left, Console.GetCursorPosition().Top);
            Console.Write("│");
            Console.Write(Environment.NewLine);
        });

        Console.Write("╰┯──────────────────┯──────────────┯───────────────┯─────────────┯─");
        
        Console.Write("".PadLeft((columnCount - MIN_COLUMNS) * 4, '─'));
        Console.Write("─╯");
        Console.Write(Environment.NewLine);
        Console.WriteLine(" │ `Red|↓´ next combatant │ add `Red|m´onster  │ `Red|a´dd player    │ e`Red|x´it combat │".FormatANSI());
        Console.WriteLine(" │ `Red|↑´ prev combatant │ `Red|k´ill monster │ kill `Red|p´layer   │ `Red|q´uit app    │".FormatANSI());
        Console.WriteLine(" │                  │              │ `Red|r´evive player │             │".FormatANSI());
        Console.WriteLine(" ╰──────────────────┷──────────────┷───────────────┷─────────────╯");
    }

    private void DrawCombatants()
    {
        var top = 4;
        var initiativeOrder = Combat.CurrentRound.Initiatives.OrderByDescending(x => x.Initiative).ToList();
        initiativeOrder.ForEach((initiative) => {
            var combatant = Combat.Combatants.First(x => x.Id == initiative.CombatantId);
            var name = combatant.Name;
            var player = combatant.Player;
            var emptyRow = "                                                                   ";
            
            // change background current player is selected or is dead
            if (combatant.IsDead)
            {
                Console.ForegroundColor = DeadCombatantForegroundColor;
                Console.BackgroundColor = DeadCombatantBackgroundColor;
            }
            else if (initiative.CombatantId == SelectedCombatantId)
            {
                Console.ForegroundColor = SelectedForeground;
                Console.BackgroundColor = SelectedBackground;
            }

            // draw a blank row
            Console.SetCursorPosition(1, top);
            Console.WriteLine(emptyRow);
            
            // draw name
            Console.SetCursorPosition(2, top);
            Console.Write($"{name}");
            
            // draw player
            if (!string.IsNullOrWhiteSpace(player))
                Console.Write($" ({player})");
            
            // draw initiative columns
            var i = 1;
            foreach (var round in Combat.Rounds)
            {
                Console.SetCursorPosition(24 + (i * 4), top);
                Console.Write(round.Value.Initiatives.First(x => x.CombatantId == combatant.Id).Initiative);
                i++;
            }
                
            Console.ForegroundColor = DefaultForeground;
            Console.BackgroundColor = DefaultBackground;
            
            top++;
        });
    }
    
    private void SelectPrevious()
    {
        if (SelectedCombatantId == FirstCombatantId)
            return;
        
        var arr = Combat.CurrentInitiativeOrder.Select(x => x.CombatantId).ToArray();
        var ix = Array.IndexOf(arr, SelectedCombatantId);
        SelectedCombatantId = arr[ix - 1];
        
        var combatant = Combat.Combatants.First(x => x.Id == SelectedCombatantId);

        // keep it on the current selected combatant if the previous one is dead
        if (combatant.IsDead)
        {
            if (SelectedCombatantId == FirstCombatantId)
                SelectedCombatantId = arr[ix];
            else
                SelectedCombatantId = arr[ix - 2];
        }
        
        SelectedCombatantChanged?.Invoke(this, new SelectedCombatantChangedEventArgs(SelectedCombatantId));
        //Redraw();
    }

    private void SelectNext()
    {
        // trigger overflow, which will create a new round
        if (SelectedCombatantId == LastCombatantId || SelectedCombatantId == FirstCombatantId)
        {
            var direction = OverflowDirection.Down;
            if (SelectedCombatantId == FirstCombatantId)
                direction = OverflowDirection.Up;
            
            Overflow?.Invoke(this, new ScreenOverflowEventArgs(direction));
            return;
        } 
            

        var arr = Combat.CurrentInitiativeOrder.Select(x => x.CombatantId).ToArray();
        var ix = Array.IndexOf(arr, SelectedCombatantId);
        SelectedCombatantId = arr[ix + 1];
        
        var combatant = Combat.Combatants.First(x => x.Id == SelectedCombatantId);

        // move to the next combatant if the current one is dead
        if (combatant.IsDead)
        {
            SelectNext();
        }
        else
        {
            SelectedCombatantChanged?.Invoke(this, new SelectedCombatantChangedEventArgs(SelectedCombatantId));
            //Redraw();
        }
    }
    
    // private void Redraw()
    // {
    //     ClearErrors();
    //     Console.SetCursorPosition(0, 4);
    //
    //     //order the rounds in ascending order
    //     var rounds = new SortedDictionary<int, List<CombatantInitiative>>();
    //
    //     foreach (var round in Combat.Rounds)
    //         rounds.Add(round.Key, round.Value);
    //
    //     //sort combatants by initiative order in current round
    //     var currentRound = rounds[Combat.CurrentRound];
    //     currentRound = currentRound.OrderByDescending(x => x.Initiative).ToList();
    //
    //     foreach (var initiativeResult in currentRound)
    //     {
    //         //DrawLine(initiativeResult);
    //     }
    //     
    //     Console.WriteLine("╰┯──────────────────┯──────────────┯───────────────┯─────────────┯───╯");
    //     actionLineTop = Console.GetCursorPosition().Top;
    //     Console.WriteLine(" │ `Red|↓´ next combatant │ add `Red|m´onster  │ `Red|a´dd player    │ e`Red|x´it combat │".FormatANSI());
    //     Console.WriteLine(" │ `Red|↑´ prev combatant │ `Red|k´ill monster │ kill `Red|p´layer   │ `Red|q´uit app    │".FormatANSI());
    //     Console.WriteLine(" │                  │              │ `Red|r´evive player │             │".FormatANSI());
    //     Console.WriteLine(" ╰──────────────────┷──────────────┷───────────────┷─────────────╯");
    // }

    // private void DrawLine(CombatantInitiative initiative)
    // {
    //     var pos = Console.GetCursorPosition();
    //     Console.ForegroundColor = DefaultForeground;
    //     Console.BackgroundColor = DefaultBackground;
    //     Console.Write("│                                                                    │");
    //
    //     var combatant = Combat.Combatants.First(x => x.Id == initiative.CombatantId);
    //     var roundInitiativeHistory = Combat.GetRoundHistory(combatant.Id);
    //
    //     var name = combatant.Name;
    //     var player = combatant.Player;
    //     var emptyRow = "                                                                   ";
    //         
    //     // change background current player is selected or is dead
    //     if (combatant.IsDead)
    //     {
    //         Console.ForegroundColor = DeadCombatantForegroundColor;
    //         Console.BackgroundColor = DeadCombatantBackgroundColor;
    //     }
    //     else if (initiative.CombatantId == SelectedCombatantId)
    //     {
    //         Console.ForegroundColor = SelectedForeground;
    //         Console.BackgroundColor = SelectedBackground;
    //     }
    //     
    //     // draw a blank row
    //     Console.SetCursorPosition(2, pos.Top);
    //     Console.WriteLine(emptyRow);
    //
    //     // draw name
    //     Console.SetCursorPosition(2, pos.Top);
    //     Console.Write($"{name}");
    //     
    //     // draw player
    //     if (!string.IsNullOrWhiteSpace(player))
    //         Console.Write($" ({player})");
    //
    //     // draw initiative columns
    //     var i = 1;
    //     foreach (var initiativeValue in roundInitiativeHistory)
    //     {
    //         Console.SetCursorPosition(24 + (i * 4), pos.Top);
    //         Console.Write(initiativeValue);
    //         i++;
    //     }
    //         
    //     Console.ForegroundColor = DefaultForeground;
    //     Console.BackgroundColor = DefaultBackground;
    //         
    //     Console.Write(Environment.NewLine);
    // }
    
    private void DrawError(Error error)
    {
        switch (error)
        {
            case Error.NoMonstersToKill:
                Console.SetCursorPosition(65, actionLineTop);
                Console.ForegroundColor = ErrorForeground;
                Console.BackgroundColor = ErrorBackground;
                Console.Write(" NO MONSTERS ");
                Console.SetCursorPosition(65, actionLineTop + 1);
                Console.Write("  TO KILL!!  ");
                break;
        }

        Console.ForegroundColor = DefaultForeground;
        Console.BackgroundColor = DefaultBackground;
    }

    private void ClearErrors()
    {
        Console.SetCursorPosition(64, actionLineTop);
        Console.Write("               ");
        Console.SetCursorPosition(64, actionLineTop + 1);
        Console.Write("               ");
    }
    
    private void CombatDataChanged(object? sender, CombatDataChangedEventArgs e)
    {
        switch (e.ChangeType)
        {
            case CombatDataChangeType.MonsterAdded:
                DrawHeader();
                DrawShell();
                DrawCombatants();
                break;
        }
    }

    private void KillCancel(object? sender, System.EventArgs e)
    {
        Show();
    }

    private void KillCombatant(object? sender, KillEventArgs e)
    {
        Combat.KillCombatant(e.Combatant);
    }
    
    private enum Error
    {
        NoMonstersToKill
    }
    
    public event EventHandler<ExitEventArgs>? Exit;
    public event EventHandler<ScreenOverflowEventArgs>? Overflow;
    public event EventHandler<SelectedCombatantChangedEventArgs>? SelectedCombatantChanged;
}
