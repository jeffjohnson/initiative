namespace Initiative.Classes.Screens.EventArgs;

public class TextCursorPositionChangedEventArgs
{
    public int TextCursorPosition { get; private set; }
    public int CursorLeft { get; private set; }
    public int CursorTop { get; private set; }

    public char[] Value { get; private set; }

    public TextCursorPositionChangedEventArgs(int textCursorPosition, int cursorLeft, int cursorTop, char[] value)
    {
        TextCursorPosition = textCursorPosition;
        CursorLeft = cursorLeft;
        CursorTop = cursorTop;
        Value = value;
    }
}