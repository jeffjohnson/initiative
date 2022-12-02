namespace Initiative.Classes.Screens.EventArgs;

public class SpecifyCombatantEventArgs : System.EventArgs
{
    public Combatant Combatant { get; set; }

    public SpecifyCombatantEventArgs(Combatant combatant)
    {
        Combatant = combatant;
    }
}