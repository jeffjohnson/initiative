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
    
    public List<CombatantInitiative> CurrentInitiativeOrder { get; set; }
    
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
        rounds.Last().Value.AddCombatant(combatant);
        
        DataChanged?.Invoke(this, new CombatDataChangedEventArgs(
            combatant.IsMonster ? CombatDataChangeType.MonsterAdded : CombatDataChangeType.PlayerAdded,
            combatant.Id));
    }

    public void KillCombatant(Combatant combatant)
    {
        Combatants.First(x => x.Id == combatant.Id).Kill();
        
        DataChanged?.Invoke(this, new CombatDataChangedEventArgs(
            combatant.IsMonster ? CombatDataChangeType.MonsterKilled : CombatDataChangeType.PlayerKilled,
            combatant.Id));
    }

    public void RevivePlayer(Combatant combatant)
    {
        Combatants.First(x => x.Id == combatant.Id).Revive();
        
        DataChanged?.Invoke(this, new CombatDataChangedEventArgs(
            CombatDataChangeType.PlayerRevived,
            combatant.Id));
    }
    
    public void StartCombat()
    {
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
        DataChanged?.Invoke(this, new CombatDataChangedEventArgs(CombatDataChangeType.PreviousRound));
    }

    // public string[] GetRoundHistory(int combatantId)
    // {
    //     var result = new List<string>();
    //     foreach (var round in rounds)
    //     {
    //         var value = round.Value.First(x => x.CombatantId == combatantId).Initiative;
    //         result.Add(value == 0 ? " —" : value.ToString().PadLeft(2, ' '));
    //     }
    //
    //     return result.ToArray();
    // }
    
    
    public void HandleOverflow(object? sender, ScreenOverflowEventArgs e)
    {
        if (e.Direction == OverflowDirection.Down)
            NextRound();
        else if (e.Direction == OverflowDirection.Up)
            PreviousRound();
    }

    public event EventHandler<CombatDataChangedEventArgs>? DataChanged;
}