using System;
using TextSpireCS.Model.Card;

namespace TextSpireCS.Model.Creature;

// Represents the player character with a Name, current and max HP, and their deck of cards.
public sealed class Player {
    public string Name { get; }
    public int Hp { get; private set; }
    public int MaxHp { get; }
    public Deck Deck { get; }

    public int Armor { get; private set; }

    public Player(string name, int maxHp, Deck deck) {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or blank.", nameof(name));
        if (maxHp <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxHp), "Max HP must be positive.");
        Deck = deck ?? throw new ArgumentNullException(nameof(deck));

        Name = name;
        MaxHp = maxHp;
        Hp = maxHp;
    }

    public bool IsDead => Hp <= 0;

    public void AddArmor(int amount) {
        if (amount <= 0) return;
        Armor += amount;
    }

    public void ResetArmor() => Armor = 0;

    public void TakeDamage(int dmg) {
        dmg = Math.Max(0, dmg);

        if (Armor > 0) {
            int absorbed = Math.Min(Armor, dmg);
            Armor -= absorbed;
            dmg -= absorbed;
        }

        if (dmg > 0) {
            Hp = Math.Max(0, Hp - dmg);
        }
    }

    public void Heal(int amount) {
        if (amount < 0) amount = 0;
        Hp = Math.Min(MaxHp, Hp + amount);
    }
}
