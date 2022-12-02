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
    private const int COLUMN_WIDTH = 4;
    private Combat Combat { get; set; }
    private int FirstCombatantId => Combat.CurrentInitiativeOrder.First().CombatantId;
    private int SelectedCombatantId { get; set; }
    private int LastCombatantId => Combat.CurrentInitiativeOrder.Last().CombatantId;
    private int actionLineTop = 4;
    private int rightColumn { get; set; }
    private int bottomRow { get; set; }
    
    // ctor
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
        Listen();
    }

    private void Listen()
    {
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
                    var aliveMonsters = Combat.Monsters.Where(x => x.IsDead != true);
                    if (!aliveMonsters.Any())
                    {
                        DrawError(Error.NoMonstersToKill);
                    }
                    else
                    {
                        ShowKillScreen?.Invoke(this, new ShowKillScreenEventArgs(CombatantType.Monster));
                    }

                    break;

                case ConsoleKey.M:
                    ShowAddMonsterScreen?.Invoke(this, System.EventArgs.Empty);
                    break;

                case ConsoleKey.N:
                    StartNewCombat?.Invoke(this, new NewCombatEventArgs()
                    {
                        PCs = Combat.PCs
                    });
                    break;

                case ConsoleKey.P:
                    var alivePCs = Combat.PCs.Where(x => x.IsDead != true);
                    if (!alivePCs.Any())
                    {
                        DrawError(Error.NoPCsToKill);
                    }
                    else
                    {
                        ShowKillScreen?.Invoke(this, new ShowKillScreenEventArgs(CombatantType.PC));
                    }

                    break;

                case ConsoleKey.Q:
                    Console.Clear();
                    var args = new ExitEventArgs()
                    {
                        PCs = Combat.PCs
                    };

                    Exit?.Invoke(this, args);
                    break;

                case ConsoleKey.R:
                    var deadPCs = Combat.PCs.Where(x => x.IsDead);
                    if (!deadPCs.Any())
                    {
                        DrawError(Error.NoPCsToRevive);
                    }
                    else
                    {
                        ShowRevivePCScreen?.Invoke(this, System.EventArgs.Empty);
                    }

                    break;
                
                default:
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
            Console.SetCursorPosition(22 + i * COLUMN_WIDTH, 1);
            Console.Write("───┬");
            
            Console.SetCursorPosition(22 + i * COLUMN_WIDTH, 2);
            Console.Write(i.FourPadded());
            
            Console.SetCursorPosition(22 + i * COLUMN_WIDTH, 3);
            Console.Write("────");
        }
        
        var pos = Console.GetCursorPosition();
        Console.SetCursorPosition(23 + (columnCount * COLUMN_WIDTH) / 2, 1);
        
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
        
        var left = 28 + (columnCount * COLUMN_WIDTH);
        
        Combat.CurrentRound.Initiatives.ForEach((x) => { 
            Console.Write("│");
            Console.SetCursorPosition(left, Console.GetCursorPosition().Top);
            Console.Write("│");
            Console.Write(Environment.NewLine);
        });

        Console.Write("╰┯──────────────────┯──────────────┯───────────┯─────────────┯─");
        
        Console.Write("".PadLeft((columnCount - MIN_COLUMNS) * COLUMN_WIDTH, '─'));
        Console.Write("─────╯");
        
        // store the furthest right position
        rightColumn = Console.GetCursorPosition().Left;
        
        Console.Write(Environment.NewLine);
        Console.WriteLine(" │ `#c81e64|↓´ next combatant │ add `#c81e64|m´onster  │ `#c81e64|a´dd pc    │ `#c81e64|n´ew combat  │".FormatANSI());
        Console.WriteLine(" │ `#c81e64|↑´ prev combatant │ `#c81e64|k´ill monster │ kill `#c81e64|p´c   │ `#c81e64|q´uit app    │".FormatANSI());
        Console.WriteLine(" │                  │              │ `#c81e64|r´evive pc │             │".FormatANSI());
        Console.WriteLine(" ╰──────────────────┷──────────────┷───────────┷─────────────╯");

        bottomRow = Console.GetCursorPosition().Top;
    }

    private void DrawCombatants()
    {
        ClearErrors();
        var top = 4;
        var initiativeOrder = Combat.CurrentRound.Initiatives.OrderByDescending(x => x.Initiative).ToList();
        initiativeOrder.ForEach((initiative) => {
            var combatant = Combat.Combatants.First(x => x.Id == initiative.CombatantId);
            var name = combatant.Name;
            var player = combatant.Player;
            var emptyRow = "".PadLeft(rightColumn - 2, ' ');
            
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
            
            // draw initiative columns for this player
            var i = 1;
            foreach (var round in Combat.Rounds)
            {
                var initiativeRoll = round.Value.Initiatives
                    .First(x => x.CombatantId == combatant.Id)
                    .Initiative.ToString();

                if (initiativeRoll == "0")
                    initiativeRoll = "—";
                
                Console.SetCursorPosition(24 + (i * COLUMN_WIDTH), top);
                Console.Write(initiativeRoll.PadLeft(2, ' '));
                i++;
            }
                
            Console.ForegroundColor = DefaultForeground;
            Console.BackgroundColor = DefaultBackground;
            
            top++;
        });
    }
    
    private void SelectPrevious()
    {
        // trigger overflow, which will create a new round
        if (SelectedCombatantId == FirstCombatantId)
        {
            if (Combat.Rounds.First().Key != Combat.CurrentRound.Number)
            {
                if (Combat.CurrentRound.Number > 10)
                {
                    ConsoleHelper.ClearColumns(rightColumn - COLUMN_WIDTH, rightColumn, bottomRow);
                }
                
                Overflow?.Invoke(this, new ScreenOverflowEventArgs(OverflowDirection.Up));
            }

            return;
        } 
        
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
        DrawCombatants();
    }

    private void SelectNext()
    {
        // trigger overflow, which will create a new round
        if (SelectedCombatantId == LastCombatantId)
        {
            Overflow?.Invoke(this, new ScreenOverflowEventArgs(OverflowDirection.Down));
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
            DrawCombatants();
        }
    }
    
    private void DrawError(Error error)
    {
        ClearErrors();
        var errorText = "";
        switch (error)
        {
            case Error.NoMonstersToKill:
                Console.SetCursorPosition(0, bottomRow);
                Console.ForegroundColor = ErrorForeground;
                Console.BackgroundColor = DefaultBackground;
                Console.WriteLine(" ╭─────────────────────────╮");
                Console.WriteLine(" │   NO MONSTERS TO KILL! │");
                Console.WriteLine(" ╰─────────────────────────╯");
                break;
            
            case Error.NoPCsToRevive:
                Console.SetCursorPosition(0, bottomRow);
                Console.ForegroundColor = ErrorForeground;
                Console.BackgroundColor = DefaultBackground;
                Console.WriteLine(" ╭─────────────────────╮");
                Console.WriteLine(" │   NO PCS ARE DEAD! │");
                Console.WriteLine(" ╰─────────────────────╯");
                break;
        }

        Console.ForegroundColor = DefaultForeground;
        Console.BackgroundColor = DefaultBackground;
    }

    private void ClearErrors()
    {
        Console.SetCursorPosition(0, bottomRow);
        Console.WriteLine("".PadLeft(rightColumn, ' '));
        Console.WriteLine("".PadLeft(rightColumn, ' '));
        Console.WriteLine("".PadLeft(rightColumn, ' '));
    }
    
    private void CombatDataChanged(object? sender, CombatDataChangedEventArgs e)
    {
        switch (e.ChangeType)
        {
            case CombatDataChangeType.MonsterAdded:
                DrawShell();
                DrawCombatants();
                break;
            
            case CombatDataChangeType.NewRound:
                if (Combat.Rounds.Count > 10)
                {
                    DrawHeader();
                    DrawShell();
                }

                SelectedCombatantId = Combat.CurrentRound.Initiatives
                    .OrderByDescending(x => x.Initiative)
                    .First().CombatantId;
                
                DrawCombatants();
                break;
            
            case CombatDataChangeType.PreviousRound:
                DrawHeader();
                DrawShell();
                DrawCombatants();
                break;
            
            case CombatDataChangeType.MonsterKilled:
            case CombatDataChangeType.PCKilled:
            case CombatDataChangeType.PCAdded:
            case CombatDataChangeType.PCRevived:
                DrawHeader();
                DrawShell();
                DrawCombatants();
                break;
        }
    }

    private enum Error
    {
        NoMonstersToKill,
        NoPCsToKill,
        NoPCsToRevive
    }
    
    public event EventHandler<ExitEventArgs>? Exit;
    public event EventHandler<NewCombatEventArgs>? StartNewCombat; 
    public event EventHandler<ScreenOverflowEventArgs>? Overflow;
    public event EventHandler<ShowKillScreenEventArgs>? ShowKillScreen;
    public event EventHandler? ShowRevivePCScreen;
    public event EventHandler? ShowAddMonsterScreen;
    public event EventHandler<SelectedCombatantChangedEventArgs>? SelectedCombatantChanged;
}
