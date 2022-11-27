using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Initiative.Classes.Screens.EventArgs;

namespace Initiative.Classes;

public class Combat
{
    private SortedDictionary<int, List<CombatantInitiative>> rounds = new SortedDictionary<int, List<CombatantInitiative>>();
    private readonly List<Combatant> combatants = new List<Combatant>();
    private readonly Identity identity = new Identity();
    public int CurrentRound { get; private set; }
    public int FirstCombatantId => CurrentInitiativeOrder.First().CombatantId;
    public int CurrentCombatantId { get; private set; }
    
    public int LastCombatantId => CurrentInitiativeOrder.Last().CombatantId;
    public List<CombatantInitiative> CurrentInitiativeOrder { get; set; }
    public SortedDictionary<int, List<CombatantInitiative>> Rounds => rounds;
    
    public Combat(IEnumerable<Combatant> combatants)
    {
        this.combatants = combatants.ToList();
        
        foreach (var combatant in this.combatants)
            combatant.Id = identity.Next;
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

    public void KillCombatant(Combatant combatant)
    {
        combatants.First(x => x.Id == combatant.Id).Kill();
        DataChanged?.Invoke(this, new CombatDataChangedEventArgs(CurrentCombatantId));
    }
    
    public void StartCombat()
    {
        CurrentRound = 0;
        NextRound();
    }

    private void NextRound()
    {
        CurrentInitiativeOrder = combatants.Select(combatant => new CombatantInitiative(combatant.Id, combatant.RollInitiative())).ToList();

        RemoveDuplicates();

        CurrentInitiativeOrder = CurrentInitiativeOrder.OrderByDescending(x => x.Initiative).ToList();
        
        CurrentCombatantId = FirstCombatantId;
        
        CurrentRound++;
        rounds.Add(CurrentRound, CurrentInitiativeOrder);
        DataChanged?.Invoke(this, new CombatDataChangedEventArgs(CurrentCombatantId));
    }

    public string[] GetRoundHistory(int combatantId)
    {
        var result = new List<string>();
        foreach (var round in rounds)
        {
            var value = round.Value.First(x => x.CombatantId == combatantId).Initiative;
            result.Add(value == 0 ? " —" : value.ToString().PadLeft(2, ' '));
        }

        return result.ToArray();
    }
    
    private void RemoveDuplicates()
    {
        var rand = new Random();

        while (HasDuplicates(CurrentInitiativeOrder))
        {
            var duplicates = CurrentInitiativeOrder.GroupBy(x => x.Initiative).Where(x => x.Skip(1).Any());
            foreach (var grouping in duplicates)
            {
                // 0 means that their initiative is 0 because they are dead
                if (grouping.Key == 0)
                    continue;
                
                var rolloff = grouping.ToDictionary(x => x.CombatantId, x => rand.Next());

                rolloff = rolloff.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
                
                var i = 0;
                foreach (var rolloffKey in rolloff.Keys)
                {
                    CurrentInitiativeOrder.First(x => x.CombatantId == rolloffKey).Initiative += i;
                    i++;
                }
            }
        }
    }
    
    private static bool HasDuplicates(List<CombatantInitiative> initiativeEntries)
    {
        var aliveEntries = initiativeEntries.Where(x => x.Initiative != 0).ToList();
        var duplicateEntries = aliveEntries.Select(x => x.Initiative).Distinct().Count();
        
        return aliveEntries.Count != duplicateEntries;
    }
    
    public void HandleOverflow(object? sender, EventArgs e)
    {
        NextRound();
    }

    public event EventHandler<CombatDataChangedEventArgs>? DataChanged;
}