using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TextSpireCS.Model.Card;
using TextSpireCS.Model.Item;
using TextSpireCS.Persist;

namespace TextSpireCS.Util;

public static class SaveManager {
    // Store next to the executable.
    private static readonly string SavePath =
        Path.Combine(AppContext.BaseDirectory, "save.json");

    // Keep the old method names to avoid touching Program.cs
    public static bool exists() => File.Exists(SavePath);

    public static SaveGame read() {
        // Exceptions are ignored since Program.cs already catches & reports "Save corrupt"
        var json = File.ReadAllText(SavePath);
        // Case insensitive
        var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<SaveGame>(json, opts) ?? new SaveGame();
    }

    public static void write(
        int floor, int strikeBonus, int defenseBonus,
        int dmg, int delay, int hp, int slimeCount,
        Deck deck, Inventory inventory) {
        if (deck is null) throw new ArgumentNullException(nameof(deck));
        if (inventory is null) throw new ArgumentNullException(nameof(inventory));
        List<Card> deckSnapshot = deck.GetAllCards();

        var save = new SaveGame {
            floor = floor,
            strikeBonus = strikeBonus,
            defenseBonus = defenseBonus,
            dmg = dmg,
            delay = delay,
            hp = hp,
            slimeCount = slimeCount,
            deck = deckSnapshot,
            potions = inventory.GetPotions().ToList(),
            weapons = inventory.GetWeapons().ToList()
        };

        var opts = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(save, opts);

        Directory.CreateDirectory(Path.GetDirectoryName(SavePath)!);

        var tmp = SavePath + ".tmp";
        File.WriteAllText(tmp, json);
        if (File.Exists(SavePath))
            File.Replace(tmp, SavePath, null);
        else
            File.Move(tmp, SavePath);
    }
}
