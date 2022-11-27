namespace Initiative.Classes.Screens.EventArgs;

public class CombatDataChangedEventArgs : System.EventArgs
{
    public int SelectedCombatantId { get; set; }
    
    public CombatDataChangedEventArgs() { }

    public CombatDataChangedEventArgs(int selectedCombatantId)
    {
        SelectedCombatantId = selectedCombatantId;
    }
}