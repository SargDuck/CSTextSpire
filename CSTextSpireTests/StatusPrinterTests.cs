using System;
using System.Collections.Generic;
using System.IO;
using TextSpireCS.Engine;
using TextSpireCS.Model.Cards;
using TextSpireCS.Model.Creature;
using TextSpireCS.Model.Item;
using TextSpireCS.UI;
using Xunit;

namespace TextSpireCS.Tests;

public class StatusPrinterTests {
    [Fact]
    public void PrintIncludesHpDeckPotionsWeapons() {
        var deck = new Deck(new[] { new Card("Strike", 6), new Card("Defend", 0) });
        var player = new Player("Hero", 30, deck);
        var enemies = new List<Enemy>();
        var ctx = new CombatContext(player, enemies, 0);

        var inv = new Inventory();
        inv.AddPotion(new Potion("Heal", 10));
        inv.AddWeapon(new Weapon("Dagger", 1));

        var sw = new StringWriter();
        var old = Console.Out;
        Console.SetOut(sw);

        try {
            StatusPrinter.Print(ctx, inv);
        }
        finally {
            Console.SetOut(old);
        }

        var text = sw.ToString();
        Assert.Contains("HP", text);
        Assert.Contains("Deck size", text);
        Assert.Contains("Potions", text);
        Assert.Contains("Weapons", text);
    }

    [Fact]
    public void PrintShowsPlayerStatsAndInventory() {
        var deck = new Deck(new[] { new Card("Strike", 6), new Card("Defend", 0) });
        var player = new Player("Hero", 20, deck);
        var enemies = new List<Enemy>();
        var ctx = new CombatContext(player, enemies, 0);
        var inv = new Inventory();
        inv.AddPotion(new Potion("Heal", 5));
        inv.AddWeapon(new Weapon("Sword", 2));

        var sw = new StringWriter();
        Console.SetOut(sw);

        StatusPrinter.Print(ctx, inv);
        var output = sw.ToString();

        Assert.Contains("HP 20 / 20", output);
        Assert.Contains("Deck size", output);
        Assert.Contains("Potions   : 1", output);
        Assert.Contains("Weapons   : 1  (bonus +2)", output);
    }
}
