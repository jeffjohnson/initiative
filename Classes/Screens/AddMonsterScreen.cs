using System.Runtime.CompilerServices;
using ANSIConsole;
using Initiative.Classes.Screens.EventArgs;

namespace Initiative.Classes.Screens;

public class AddMonsterScreen : ScreenBase
{
  private Combat Combat { get; set; }
  private Combatant NewMonster { get; set; }
  private TextInput Name { get; set; }
  private TextInput InitiativeBonus { get; set; }
  
  private TextInput CurrentInput { get; set; }

  public AddMonsterScreen(Combat combat)
  {
    Combat = combat;
    NewMonster = new Combatant();
  }

  public void Show()
  {
      DrawScreen();
  }

  private void DrawScreen()
  {
      Reset();
      Console.ForegroundColor = DefaultForeground;
      Console.BackgroundColor = DefaultBackground;
      Console.WriteLine("╭─────────────╮");
      Console.WriteLine("│ Add Monster ╰────────────────────────╮");
      Console.WriteLine("├──────────────────────────────────────┤");
      Console.WriteLine("│ Name:                                │");
      Console.WriteLine("│ Initiative Bonus:                    │");
      Console.WriteLine("╰┯─────────┯───────────────┯───────────╯");
      Console.WriteLine(" │ `#c81e64|⌃c´ancel │ `#c81e64|⌃s´ave monster │".FormatANSI());
      Console.WriteLine(" ╰─────────┷───────────────╯");
      
      Name = new TextInput(8, 3);
      Name.TextChanged += NameTextChanged;
      Name.KeyPressed += InputKeyPressed;
      Name.LostFocus += InputLostFocus;
      
      InitiativeBonus = new TextInput(20, 4);
      InitiativeBonus.TextChanged += InitiativeBonusTextChanged;
      InitiativeBonus.KeyPressed += InputKeyPressed;
      InitiativeBonus.LostFocus += InputLostFocus;
      
      CurrentInput = Name;
      Name.Focus();
  }

  private void SwitchInput()
  {
      if (CurrentInput == Name)
      {
          CurrentInput = InitiativeBonus;
          InitiativeBonus.Focus();
          
      }
      else if (CurrentInput == InitiativeBonus)
      {
          CurrentInput = Name;
          Name.Focus();
      }
  }

  private void NameTextChanged(object? sender, System.EventArgs e)
  {
      NewMonster.Name = new string(Name.Value.ToArray());
  }
  
  private void InitiativeBonusTextChanged(object? sender, System.EventArgs e)
  {
      var bonus = new string(InitiativeBonus.Value.ToArray()).TrimStart('+');
      if (Int32.TryParse(bonus, out var initiativeBonus))
      {
          NewMonster.InitiativeBonus = initiativeBonus;
      }
  }

  private void InputLostFocus(object? sender, KeyPressedEventArgs e)
  {
      SwitchInput();
  }
  
    private void InputKeyPressed(object? sender, KeyPressedEventArgs e)
  {
      switch (e.KeyPressed.Key)
      {
          case ConsoleKey.S:
              if ((e.KeyPressed.Modifiers & ConsoleModifiers.Control) != 0)
                  SaveMonster?.Invoke(this, new SpecifyCombatantEventArgs(NewMonster));

              break;

          case ConsoleKey.C:
              if ((e.KeyPressed.Modifiers & ConsoleModifiers.Control) != 0)
                  Cancel?.Invoke(this, System.EventArgs.Empty);

              break;
      }
  }
  
  public event EventHandler? Cancel;
  public event EventHandler<SpecifyCombatantEventArgs>? SaveMonster;
}