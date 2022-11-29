namespace Initiative.Classes.Screens.EventArgs;

public class SelectedCombatantChangedEventArgs
{
    public readonly int SelectedCombatantId;

    public SelectedCombatantChangedEventArgs(int selectedCombatantId)
    {
        SelectedCombatantId = selectedCombatantId;
    }
}