using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Initiative.Classes.Screens.EventArgs;

namespace Initiative.Classes;

public class Combat
{
    private readonly Identity identity;

    private readonly SortedDictionary<int, Round> rounds;
    public SortedDictionary<int, Round> Rounds => rounds;
    
    public List<Combatant> Combatants { get; private set; }
    public List<Combatant> Monsters => Combatants.Where(x => string.IsNullOrEmpty(x.Player)).ToList();
    public List<Combatant> PCs => Combatants.Where(x => !string.IsNullOrEmpty(x.Player)).ToList();
    
    public Round CurrentRound { get; private set; }

    public List<CombatantInitiative> CurrentInitiativeOrder =>
        CurrentRound.Initiatives.OrderByDescending(x => x.Initiative).ToList();
    
    // ctor
    public Combat(IEnumerable<Combatant> combatants)
    {
        // assign provided combatants to this combat
        Combatants = combatants.ToList();
        
        // tracks initiative for each combatant for each round
        this.rounds = new SortedDictionary<int, Round>();
        
        // assign each combatant a unique id
        this.identity = new Identity();
        foreach (var combatant in Combatants)
            combatant.Id = identity.Next;
    }

    public void AddCombatant(Combatant combatant)
    {
        if (combatant.Id == 0)
            combatant.Id = identity.Next;
        
        Combatants.Add(combatant);
        foreach (var round in rounds)
        {
            var initiative = 0;
            if (round.Key == CurrentRound.Number)
                initiative = combatant.RollInitiative();
            
            round.Value.AddCombatant(combatant, initiative);
        }
        
        DataChanged?.Invoke(this, new CombatDataChangedEventArgs(
            combatant.IsMonster ? CombatDataChangeType.MonsterAdded : CombatDataChangeType.PCAdded,
            combatant.Id));
    }

    public void KillCombatant(Combatant combatant)
    {
        Combatants.First(x => x.Id == combatant.Id).Kill();
        
        DataChanged?.Invoke(this, new CombatDataChangedEventArgs(
            combatant.IsMonster ? CombatDataChangeType.MonsterKilled : CombatDataChangeType.PCKilled,
            combatant.Id));
    }

    public void RevivePC(Combatant combatant)
    {
        Combatants.First(x => x.Id == combatant.Id).Revive();
        
        DataChanged?.Invoke(this, new CombatDataChangedEventArgs(
            CombatDataChangeType.PCRevived,
            combatant.Id));
    }
    
    public void StartCombat()
    {
        rounds.Clear();
        NextRound();
    }

    private void NextRound()
    {
        var newRound = rounds.Count() + 1;
        CurrentRound = new Round(newRound, Combatants);
        rounds.Add(newRound, CurrentRound);
        DataChanged?.Invoke(this, new CombatDataChangedEventArgs(CombatDataChangeType.NewRound));
    }

    private void PreviousRound()
    {
        var roundId = rounds.Last().Key;
        rounds.Remove(roundId);
        CurrentRound = rounds.Last().Value;
        DataChanged?.Invoke(this, new CombatDataChangedEventArgs(CombatDataChangeType.PreviousRound));
    }

    public void HandleOverflow(object? sender, ScreenOverflowEventArgs e)
    {
        if (e.Direction == OverflowDirection.Down)
            NextRound();
        else if (e.Direction == OverflowDirection.Up)
            PreviousRound();
    }

    public event EventHandler<CombatDataChangedEventArgs>? DataChanged;
}