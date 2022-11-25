using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Initiative.Classes;

public class Combat
{
    private SortedDictionary<int, List<CombatantInitiative>> rounds = new SortedDictionary<int, List<CombatantInitiative>>();
    private readonly List<Combatant> combatants = new List<Combatant>();
    private readonly Identity identity = new Identity();
    public int CurrentRound { get; private set; }
    public SortedDictionary<int, List<CombatantInitiative>> Rounds => rounds;
    
    public Combat(List<Combatant> combatants)
    {
        foreach (var combatant in combatants)
            combatant.Id = identity.Next;
        
        this.combatants = combatants;
    }

    public IEnumerable<Combatant> PCs()
    {
        return combatants.Where(x => !string.IsNullOrEmpty(x.Player));
    }

    public IEnumerable<Combatant> Monsters()
    {
        return combatants.Where(x => string.IsNullOrEmpty(x.Player));
    }

    public IEnumerable<Combatant> Combatants()
    {
        return combatants;
    }

    public void AddCombatant(Combatant combatant)
    {
        if (combatant.Id == 0)
            combatant.Id = identity.Next;
        
        combatant.RollInitiative();
        combatants.Add(combatant);
    }
    
    public void AddCombatant(string name, string player, int initiativeBonus)
    {
        AddCombatant(new Combatant(identity.Next, name, player, initiativeBonus));  
    }

    public void Start()
    {
        CurrentRound = 0;
        NextRound();
    }

    public void NextRound()
    {
        var initiative = new List<CombatantInitiative>();
        
        foreach (var combatant in combatants)
            initiative.Add(new CombatantInitiative(combatant.Id, combatant.RollInitiative()));

        while (HasDuplicates(initiative))
            RemoveDuplicates(ref initiative);
        
        CurrentRound++;
        rounds.Add(CurrentRound, initiative);
    }
    
    private void RemoveDuplicates(ref List<CombatantInitiative> initiative)
    {
        var rand = new Random();

        var initiativeDictionary = new Dictionary<int, int>();
        initiative.ForEach(x =>
        {
            initiativeDictionary.Add(x.CombatantId, x.Initiative);
        });
        
        while (HasDuplicates(initiative))
        {
            var lookup = initiativeDictionary.ToLookup(x => x.Value, x => x.Key).Where(x => x.Count() > 1);
            foreach (var grouping in lookup)
            {
                var rolloff = grouping.ToDictionary(combatant => combatant, combatant => rand.Next());

                rolloff = rolloff.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                
                var i = 0;
                foreach (var rolloffKey in rolloff.Keys)
                {
                    initiativeDictionary[rolloffKey] = initiativeDictionary[rolloffKey] + i;
                    i++;
                }
            }
        }

        foreach (var initiativeResult in initiativeDictionary)
        {
            initiative.First(x => x.CombatantId == initiativeResult.Key).Initiative = initiativeResult.Value;
        }
    }
    
    private static bool HasDuplicates(List<CombatantInitiative> initiative)
    {
        return initiative.Count != initiative.Select(x => x.Initiative).Distinct().Count();
    }
}