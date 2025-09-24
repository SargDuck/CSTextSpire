using System.Collections.Generic;
using TextSpireCS.Model.Cards;
using TextSpireCS.Model.Item;
using TextSpireCS.Model.Misc;

namespace TextSpireCS.Persist;

// Collection of properties that capture the game state.
// Used by SaveManager to write and read.
public sealed class SaveGame {
    public int floor { get; set; }
    public int strikeBonus { get; set; }
    public int defenseBonus { get; set; }
    public int dmg { get; set; }
    public int delay { get; set; }
    public int hp { get; set; }
    public int slimeCount { get; set; }
    public List<Card> deck { get; set; } = new();
    public List<Potion> potions { get; set; } = new();
    public List<Weapon> weapons { get; set; } = new();
    public int gold { get; set; } = 0;
    public string heroClass { get; set; } = "Warrior";
    public List<string> achievements { get; set; } = new();
    public List<RunEntry>? runs { get; set; }
    public List<Relic> relics { get; set; } = new();
}
