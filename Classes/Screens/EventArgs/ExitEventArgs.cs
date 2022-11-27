namespace Initiative.Classes.Screens.EventArgs;

public class ExitEventArgs : System.EventArgs
{
    public IEnumerable<Combatant> PCs { get; set; }
}