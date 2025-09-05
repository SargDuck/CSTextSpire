using System.Collections.Generic;
using TextSpireCS.Model.Card;
using TextSpireCS.Model.Item;

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
}
