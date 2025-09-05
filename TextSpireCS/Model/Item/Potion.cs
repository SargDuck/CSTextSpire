using System.Text.Json.Serialization;
using TextSpireCS.Model.Creature;

namespace TextSpireCS.Model.Item;

// Represents a consumable item that can heal the player.
public sealed class Potion {
    public string Name { get; set; } = "";
    public int Potency { get; set; }

    public Potion() { }

    public Potion(string name, int potency) {
        Name = name;
        Potency = potency;
    }

    public void use(Player player, int healValue) {
        player.Heal(healValue);
    }
}
