namespace Initiative.Classes;

public class Round
{
    public readonly int Number;
    public List<CombatantInitiative> Initiatives { get; private set; }
    
    public Round(int roundNumber, List<Combatant> combatants)
    {
        Number = roundNumber;
        Initiatives = combatants
            .Select(combatant => new CombatantInitiative(combatant.Id, combatant.RollInitiative()))
            .OrderByDescending(x => x.Initiative)
            .ToList();
        
        RemoveDuplicateInitiatives();
    }

    public void AddCombatant(Combatant combatant, int initiative)
    {
        Initiatives.Add(new CombatantInitiative(combatant.Id, initiative));
        Initiatives = Initiatives
            .OrderByDescending(x => x.Initiative)
            .ToList();
        
        RemoveDuplicateInitiatives();
    }
    
    private void RemoveDuplicateInitiatives()
    {
        var rand = new Random();

        while (DuplicateInitiativesExist())
        {
            var duplicates = Initiatives
                .GroupBy(x => x.Initiative)
                .Where(x => x.Skip(1).Any());
            
            foreach (var grouping in duplicates)
            {
                // 0 means that their initiative is 0 because they are dead
                if (grouping.Key == 0)
                    continue;
                
                var rolloff = grouping
                    .ToDictionary(x => x.CombatantId, x => rand.Next());

                rolloff = rolloff
                    .OrderBy(x => x.Value)
                    .ToDictionary(x => x.Key, x => x.Value);
                
                var i = 0;
                foreach (var rolloffKey in rolloff.Keys)
                {
                    Initiatives.First(x => x.CombatantId == rolloffKey).Initiative += i;
                    i++;
                }
            }
        }
    }
    
    private bool DuplicateInitiativesExist()
    {
        var aliveEntries = Initiatives.Where(x => x.Initiative != 0).ToList();
        var duplicateEntries = aliveEntries.Select(x => x.Initiative).Distinct().Count();
        
        return aliveEntries.Count != duplicateEntries;
    }
    
}