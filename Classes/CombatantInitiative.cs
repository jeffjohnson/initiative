namespace Initiative.Classes;

public class CombatantInitiative
{
    public int CombatantId { get; set; }
    public int Initiative { get; set; }

    public CombatantInitiative(int combatantId, int initiative)
    {
        CombatantId = combatantId;
        Initiative = initiative;
    }
}