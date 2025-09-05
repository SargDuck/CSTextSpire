using System;
using System.Text.Json.Serialization;

namespace TextSpireCS.Model.Item;

// Readonly struct for damage values.
public readonly struct Damage {
    public int Base { get; }
    public int Bonus { get; }
    public int Total => Base + Bonus;

    public Damage(int b, int bonus) {
        Base = b;
        Bonus = bonus;
    }

    public static Damage operator +(Damage a, Damage b) =>
        new(a.Base + b.Base, a.Bonus + b.Bonus);

    // Implicit conversion to int. You can pass a Damage when an int is expected.
    public static implicit operator int(Damage d) => d.Total;
}

public sealed class Weapon {
    public string Name { get; set; } = "";
    public int DamageBonus { get; set; }

    [JsonIgnore]
    public Damage Damage => new Damage(0, DamageBonus);

    public Weapon() { }

    public Weapon(string name, int bonus) {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Weapon name cannot be null, empty, or whitespace.", nameof(name));

        if (bonus < 0)
            throw new ArgumentOutOfRangeException(nameof(bonus), bonus, "Damage bonus must be non-negative.");

        Name = name;
        DamageBonus = bonus;
    }

    // Convenience Method that returns an int via implicit conversion from Damage which equals DamageBonus
    public int Hit() => Damage;
}