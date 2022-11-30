namespace Initiative.Classes.Screens.EventArgs;

public class CombatDataChangedEventArgs : System.EventArgs
{
    public CombatDataChangeType ChangeType { get; set; }
    public int? Id { get; set; }
    
    public CombatDataChangedEventArgs() { }

    public CombatDataChangedEventArgs(CombatDataChangeType changeType, int? id = null)
    {
        ChangeType = changeType;
        Id = id;
    }
}

public enum CombatDataChangeType
{
    NewCombat,
    NewRound,
    PreviousRound,
    PCAdded,
    PCKilled,
    PCRevived,
    MonsterAdded,
    MonsterKilled
}