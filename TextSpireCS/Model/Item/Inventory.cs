using System.Collections.Generic;
using TextSpireCS.Model.Misc;

namespace TextSpireCS.Model.Item;

// A container for two collections: list of potions (consumable) objects and a list of weapon (permanent) objects.
public sealed class Inventory {
    // Stores _potions and _weapons as List<T>s 
    private readonly List<Potion> _potions = new();
    private readonly List<Weapon> _weapons = new();
    private readonly List<Relic> _relics = new();

    // Returns a mutable list of potions.
    public List<Potion> GetPotions() => _potions;

    // Returns a mutable list of weapons.
    public List<Weapon> GetWeapons() => _weapons;
    public List<Relic> GetRelics() => _relics;

    // Adds a potion to the collection.
    public void AddPotion(Potion p) => _potions.Add(p);

    // Adds a weapon to the collection.
    public void AddWeapon(Weapon w) => _weapons.Add(w);
    public void AddRelic(Relic r) => _relics.Add(r);
}