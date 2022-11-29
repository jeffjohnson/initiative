using System;
using System.Text.Json.Serialization;
namespace Initiative.Classes;

public class Combatant
{
    [JsonIgnore]
    public int Id { get; set; }
    
    [JsonIgnore]
    public bool IsDead { get; set; }

    [JsonIgnore]
    public bool IsMonster => string.IsNullOrWhiteSpace(Player);

    [JsonPropertyName("initiative")]
    public int InitiativeBonus { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("player")]
    public string Player { get; set; }

    public Combatant() {}

    public Combatant(int id, string name, string player, int initiativeBonus)
    {
        Id = id;
        Name = name;
        Player = player;
        InitiativeBonus = initiativeBonus;
    }

    public int RollInitiative()
    {
        // set dead monsters to 0
        if (IsDead && string.IsNullOrWhiteSpace(Player))
            return 0;
        
        var result = new Random().Next(1, 21) + InitiativeBonus;
        
        if (result < 1)
            return 1;

        return result;
    }

    public void Kill()
    {
        IsDead = true;
    }

    public void Revive()
    {
        IsDead = false;
    }
}