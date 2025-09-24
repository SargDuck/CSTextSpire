using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using TextSpireCS.Model.Cards;
using TextSpireCS.Model.Item;
using TextSpireCS.Persist;
using TextSpireCS.Util;
using Xunit;

namespace TextSpireCS.Tests;

public class SaveManagerTests {

    // Mirrors the same path SaveManager uses.
    private static string SaveDir => AppContext.BaseDirectory;
    private static string SavePath => Path.Combine(SaveDir, "save.json");
    private static string SaveTmpPath => SavePath + ".tmp";
    private string? _backupPath;

    public SaveManagerTests() {
        // Backup any existing save.json to avoid destroying a real save during tests
        if (File.Exists(SavePath)) {
            _backupPath = Path.Combine(SaveDir, $"save_backup_{Guid.NewGuid():N}.json");
            File.Copy(SavePath, _backupPath!, overwrite: false);
            File.Delete(SavePath);
        }
        // Clean leftover tmp
        if (File.Exists(SaveTmpPath)) File.Delete(SaveTmpPath);
    }

    ~SaveManagerTests() {
        // Restore backup if we made one
        try {
            if (File.Exists(SavePath)) File.Delete(SavePath);
            if (!string.IsNullOrEmpty(_backupPath) && File.Exists(_backupPath)) {
                File.Move(_backupPath!, SavePath);
            }
        }
        catch { }
    }

    // Buils a simple deck and forces a card into the discard pile to test SaveManager snapshots.
    private static Deck StarterDeck() {
        var d = new Deck(new[] { new Card("Strike", 6), new Card("Defend", 0) });
        var c = d.Draw();
        if (c is not null) d.Discard(c);
        return d;
    }

    private static Inventory StarterInv() {
        var inv = new Inventory();
        inv.AddPotion(new Potion("Heal", 10));
        inv.AddWeapon(new Weapon("Sword", 2));
        return inv;
    }

    [Fact]
    public void SaveFileExistence() {
        Assert.False(SaveManager.exists());
        var sg = new SaveGame { floor = 1 };
        File.WriteAllText(SavePath, JsonSerializer.Serialize(sg));
        Assert.True(SaveManager.exists());
    }

    [Fact]
    public void WriteCreatesSaveFileAndRemovesTmp() {
        var deck = StarterDeck();
        var inv = StarterInv();
        SaveManager.write(floor: 3, strikeBonus: 1, defenseBonus: 2, dmg: 4, delay: 2, hp: 20, slimeCount: 2, deck: deck, inventory: inv);
        Assert.True(File.Exists(SavePath));
        Assert.False(File.Exists(SaveTmpPath)); 
    }

    [Fact]
    public void WriteReadRoundTrip() {
        var deck = StarterDeck();
        var inv = StarterInv();
        SaveManager.write(floor: 7, strikeBonus: 2, defenseBonus: 3, dmg: 5, delay: 2, hp: 33, slimeCount: 4, deck: deck, inventory: inv);
        var sg = SaveManager.read();
        Assert.Equal(7, sg.floor);
        Assert.Equal(2, sg.strikeBonus);
        Assert.Equal(3, sg.defenseBonus);
        Assert.Equal(5, sg.dmg);
        Assert.Equal(2, sg.delay);
        Assert.Equal(33, sg.hp);
        Assert.Equal(4, sg.slimeCount);
        Assert.True(sg.deck.Count >= 2);
        Assert.Contains(sg.deck, c => c.Name == "Strike" && c.Dmg == 6);
        Assert.Contains(sg.deck, c => c.Name == "Defend" && c.Dmg == 0);
        Assert.Single(sg.potions);
        Assert.Equal("Heal", sg.potions[0].Name);
        Assert.Equal(10, sg.potions[0].Potency);
        Assert.Single(sg.weapons);
        Assert.Equal("Sword", sg.weapons[0].Name);
        Assert.Equal(2, sg.weapons[0].DamageBonus);
    }

    [Fact]
    public void ReadIsCaseInsensitive() {
        var alt = new {
            Floor = 5, StrikeBonus = 1, DefenseBonus = 1, Dmg = 4, Delay = 2, Hp = 22, SlimeCount = 3,
            Deck = new[] { new { Name = "Strike", Dmg = 6 } },
            Potions = new[] { new { Name = "Heal", Potency = 10 } },
            Weapons = new[] { new { Name = "Sword", DamageBonus = 2 } }
        };
        var json = JsonSerializer.Serialize(alt, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(SavePath, json);
        var sg = SaveManager.read();
        Assert.Equal(5, sg.floor);
        Assert.Equal(1, sg.strikeBonus);
        Assert.Equal(1, sg.defenseBonus);
        Assert.Equal(22, sg.hp);
        Assert.Equal(3, sg.slimeCount);
        Assert.Single(sg.deck);
        Assert.Single(sg.potions);
        Assert.Single(sg.weapons);
    }

    [Fact]
    public void WriteThrowsOnNullArguments() {
        var inv = StarterInv();
        var deck = StarterDeck();
        Assert.Throws<ArgumentNullException>(() => SaveManager.write(0, 0, 0, 0, 0, 0, 0, deck: null!, inventory: inv));
        Assert.Throws<ArgumentNullException>(() => SaveManager.write(0, 0, 0, 0, 0, 0, 0, deck: deck, inventory: null!));
    }
}
