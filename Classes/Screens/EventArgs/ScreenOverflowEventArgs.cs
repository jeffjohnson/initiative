namespace Initiative.Classes.Screens.EventArgs;

public class ScreenOverflowEventArgs
{
    public OverflowDirection Direction { get; set; }
    
    public ScreenOverflowEventArgs(OverflowDirection direction)
    {
        Direction = direction;
    }
}

public enum OverflowDirection
{
    Up,
    Down
}