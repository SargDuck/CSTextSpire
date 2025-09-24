using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSpireCS.Model.Cards;

namespace TextSpireCS.Model.Creature;

public enum HeroClass { Warrior, Rogue, Mage }

public static class HeroFactory {
    public static (int maxHp, Card[] deck) Create(HeroClass c) => c switch {
        HeroClass.Warrior => (32, new[] { new Card("Strike", 6), new("Strike", 6), new("Strike", 6), new("Defend", 0), new("Defend", 0) }),
        HeroClass.Rogue => (26, new[] { new Card("Strike", 6), new("Strike", 6), new("Strike", 6), new("Defend", 0) }),
        HeroClass.Mage => (28, new[] { new Card("Strike", 6), new("Strike", 6), new("Defend", 0), new("Defend", 0) }),
        _ => (30, new[] { new Card("Strike", 6), new("Defend", 0) })
    };
}

public static class HeroSelection {
    public static HeroClass Current { get; private set; } = HeroClass.Warrior;
    public static void Set(HeroClass c) => Current = c;
}