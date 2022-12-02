using System.Xml.Xsl;
using Initiative.Classes.Screens.EventArgs;

namespace Initiative.Classes.Screens;

public class TextInput
{
    public List<char> Value { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public bool IsFocused { get; set; }
    private int textCursorPosition { get; set; }
    private bool insertMode { get; set; }

    public TextInput(int x, int y)
    {
        Value = new List<char>();
        IsFocused = false;
        X = x;
        Y = y;
    }

    public void Listen()
    {
        Console.CursorVisible = true;
        
        var action = new ConsoleKeyInfo();
        while (IsFocused)
        {
            action = Console.ReadKey(true);
            KeyPressed?.Invoke(this, new KeyPressedEventArgs(action));
            
            switch (action.Key)
            {
                case ConsoleKey.UpArrow:
                case ConsoleKey.DownArrow:
                case ConsoleKey.Tab:
                case ConsoleKey.Enter:
                    IsFocused = false;
                    LostFocus?.Invoke(this, new KeyPressedEventArgs(action));
                    TextChanged?.Invoke(this, System.EventArgs.Empty);
                    break;
                
                case ConsoleKey.LeftArrow:
                    MoveScreenCursor(CursorDirection.Left);
                    break;
                
                case ConsoleKey.RightArrow:
                    MoveScreenCursor(CursorDirection.Right);
                    break;
                
                case ConsoleKey.Backspace:
                    DeleteCharacter(CursorDirection.Left);
                    TextChanged?.Invoke(this, System.EventArgs.Empty);
                    break;
                   
                case ConsoleKey.Delete:
                    DeleteCharacter(CursorDirection.Right);
                    TextChanged?.Invoke(this, System.EventArgs.Empty);
                    break;
                
                case ConsoleKey.Insert:
                    ToggleInsertMode();
                    break;
                
                case ConsoleKey.Home:
                    textCursorPosition = 0;
                    RedrawText();
                    Console.SetCursorPosition(X + textCursorPosition, Console.CursorTop);
                    TextCursorPositionChanged?.Invoke(this, new TextCursorPositionChangedEventArgs(textCursorPosition, Console.CursorLeft, Console.CursorTop, Value.ToArray()));
                    break;
                
                case ConsoleKey.End:
                    textCursorPosition = Value.Count;
                    RedrawText();
                    Console.SetCursorPosition(X + textCursorPosition, Console.CursorTop);
                    TextCursorPositionChanged?.Invoke(this, new TextCursorPositionChangedEventArgs(textCursorPosition, Console.CursorLeft, Console.CursorTop, Value.ToArray()));
                    break;
                
                case ConsoleKey.PageUp:
                case ConsoleKey.PageDown:
                case ConsoleKey.Print:
                case ConsoleKey.PrintScreen:
                case ConsoleKey.F1:
                case ConsoleKey.F2:
                case ConsoleKey.F3:
                case ConsoleKey.F4:
                case ConsoleKey.F5:
                case ConsoleKey.F6:
                case ConsoleKey.F7:
                case ConsoleKey.F8:
                case ConsoleKey.F9:
                case ConsoleKey.F10:
                case ConsoleKey.F11:
                case ConsoleKey.F12:
                    break;
                    
                default:
                    if (insertMode && textCursorPosition != Value.Count)
                        Value[textCursorPosition] = action.KeyChar;
                    else
                        Value.Insert(textCursorPosition, action.KeyChar);
                    
                    textCursorPosition += 1;
                    RedrawText();
                    Console.SetCursorPosition(X + textCursorPosition, Console.CursorTop);
                    TextChanged?.Invoke(this, System.EventArgs.Empty);
                    TextCursorPositionChanged?.Invoke(this, new TextCursorPositionChangedEventArgs(textCursorPosition, Console.CursorLeft, Console.CursorTop, Value.ToArray()));
                    break;
            }
        }

        Console.CursorVisible = false;
        Environment.Exit(0);
    }

    private void ToggleInsertMode()
    {
        if (insertMode)
        {
            insertMode = false;
        }
        else
        {
            insertMode = true;
        }
    }

    private void RedrawText()
    {
        Console.CursorVisible = false;
        Console.SetCursorPosition(X,Y);
        Console.Write("".PadLeft(Value.Count + 1));
        Console.SetCursorPosition(X,Y);
        Console.Write(new string(Value.ToArray()).Trim());
        Console.CursorVisible = true;
    }

    private (int Left, int Top) MoveScreenCursor(CursorDirection direction)
    {
        if (direction == CursorDirection.Left)
        {
            // as far left as it can go
            if (textCursorPosition == 0)
                return (X, Y);
            
            textCursorPosition -= 1;
            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
        }
        else if (direction == CursorDirection.Right)
        {
            // as far right as it can go
            if (textCursorPosition == Value.Count)
                return (X + textCursorPosition, Y);
            
            textCursorPosition += 1;
            Console.SetCursorPosition(Console.CursorLeft + 1, Console.CursorTop);
        }

        TextCursorPositionChanged?.Invoke(this, new TextCursorPositionChangedEventArgs(textCursorPosition, Console.CursorLeft, Console.CursorTop, Value.ToArray()));
        return Console.GetCursorPosition();
    }

    private void DeleteCharacter(CursorDirection direction)
    {
        if (direction == CursorDirection.Left)
        {
            if (textCursorPosition == 0)
                return;
            
            MoveScreenCursor(direction);
        } 
        else if (direction == CursorDirection.Right)
        {
            if (textCursorPosition == Value.Count)
                return;
        }
        
        
        Value.RemoveAt(textCursorPosition);
        RedrawText();

        Console.SetCursorPosition(X + textCursorPosition, Console.CursorTop);
        
        TextCursorPositionChanged?.Invoke(this, new TextCursorPositionChangedEventArgs(textCursorPosition, Console.CursorLeft, Console.CursorTop, Value.ToArray()));
    }
    
    public void Focus()
    {
        IsFocused = true;
        Console.SetCursorPosition(X + Value.Count, Y);
        Listen();
    }

    public event EventHandler? TextChanged;
    public event EventHandler<KeyPressedEventArgs>? KeyPressed;
    public event EventHandler<KeyPressedEventArgs>? LostFocus;
    public event EventHandler<TextCursorPositionChangedEventArgs> TextCursorPositionChanged;
}

public enum CursorDirection
{
    Left,
    Right
}