namespace Initiative.Classes.Screens;

public class InitiativeTracker : ScreenBase
{
    private ConsoleColor SelectedBackground => ConsoleColor.DarkCyan;
    private ConsoleColor SelectedForeground =>
    public static void Draw(Combat combat, int currentPlayerId)
    {
        Reset();
        
        Console.WriteLine("╭────────────╮");
        Console.WriteLine("│ Initiative ╰─────────┬───┬───┬───┬───┬─╼ ROUND ╾─┬───┬────┬────┬────┬────┬───╮");
        Console.WriteLine("│  Tracker             1   2   3   4   5   6   7   8   9   10   11   12   13   │");
        Console.WriteLine("├──────────────────────────────────────────────────────────────────────────────┤");

        var combatants = combat.Combatants().ToDictionary(x => x.Id, x => x);
        
        //order the rounds in ascending order
        var rounds = new SortedDictionary<int, List<CombatantInitiative>>();

        foreach (var round in combat.Rounds)
            rounds.Add(round.Key, round.Value);
        
        //sort combatants by initiative order in current round
        var currentRound = rounds[combat.CurrentRound];
        currentRound = currentRound.OrderByDescending(x => x.Initiative).ToList();

        foreach (var initiativeResult in currentRound)
        {

            // change background current player is selected
            if (initiativeResult.CombatantId == currentPlayerId)
            {
                Console.BackgroundColor = ConsoleColor.DarkCyan;
                Console.ForegroundColor = 
            }
            
            var combatant = combatants[initiativeResult.CombatantId];
            Console.Write("│");
            

                
            Console.Write($"{combatant.Name}");
            if (!string.IsNullOrWhiteSpace(combatant.Player))
                Console.Write($" ({combatant.Player})");
                
        }

        private static void DrawCombatantRowBackground(bool active)
        {
            
        }

    
}
}
