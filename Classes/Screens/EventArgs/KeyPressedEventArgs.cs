namespace Initiative.Classes.Screens.EventArgs;

public class KeyPressedEventArgs
{
    public ConsoleKeyInfo KeyPressed { get; set; }

    public KeyPressedEventArgs(ConsoleKeyInfo keyPressed)
    {
        KeyPressed = keyPressed;
    }
}