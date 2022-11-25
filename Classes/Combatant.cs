using System;
using System.Text.Json.Serialization;
namespace Initiative.Classes;

public class Combatant
{
    [JsonIgnore]
    public int Id { get; set; }
    
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
        var rnd = new Random();
        return rnd.Next(1, 21) + InitiativeBonus;
    }
}