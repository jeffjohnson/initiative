namespace Initiative.Classes.Screens.EventArgs;

public class ShowKillScreenEventArgs
{
    public CombatantType type { get; set; }

    public ShowKillScreenEventArgs(CombatantType combatantType)
    {
        type = combatantType;
    }
}