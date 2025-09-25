using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextSpireCS.Economy;
using TextSpireCS.Engine;
using TextSpireCS.Model.Cards;
using TextSpireCS.Model.Creature;
using TextSpireCS.Model.Item;
using TextSpireCS.Model.Misc;
using TextSpireCS.Model.World;
using TextSpireCS.Persist;
using TextSpireCS.UI;
using TextSpireCS.Util;

namespace TextSpireCS;

public class Program {
    // sets up the initial vars
    private int _floor = 0;
    private int _strikeBonus = 0;
    private int _defenseBonus = 0;
    private int _slimeDmg = 4;
    private int _slimeDelay = 2;
    private int _slimeHp = 20;
    private int _slimeCount = 2;

    private readonly Inventory inventory = new();
    private Deck deck;
    private Player player;

    // Main thread
    public static async Task Main(string[] args) {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var game = new Program();
        await game.RunAsync();
    }

    private async Task RunAsync() {

        // Prints a banner and a main menu.
        BannerPrinter.Print();
        while (true) {
            string choice = MainMenu();
            switch (choice) {
                case "1":
                    // New Game: choose hero
                    if (deck == null) {
                        var hero = UI.HeroSelectScreen.Choose();
                        TextSpireCS.Model.Creature.HeroSelection.Set(hero);
                        var (hp, deckCards) = TextSpireCS.Model.Creature.HeroFactory.Create(hero);
                        deck = new Deck(deckCards);
                        player = new Player(hero.ToString(), hp, deck);
                    }
                    break; // break out of switch then break out of while to start the run

                case "2":
                    if (!loadSave()) {
                        Pause("No save found. Press Enter to return to menu...");
                        continue; // stay in menu
                    }
                    Console.WriteLine("Load successful.");
                    continue; // stay in menu

                case "3":
                    tweakSettings();
                    continue; // stay in menu after tweaking

                case "5":
                    UI.ManualScreen.Show();
                    Pause("Press Enter to return to menu...");
                    continue;

                case "6":
                    UI.AchievementsScreen.Show();
                    Pause("Press Enter to return to menu...");
                    continue;

                case "7":
                    if (SaveManager.exists())
                        UI.RunHistoryScreen.Show(SaveManager.read());
                    else
                        Console.WriteLine("No history yet.");
                    Pause("Press Enter to return to menu...");
                    continue;

                case "4":
                    Console.WriteLine("Bye!");
                    return; // exit

                default:
                    Console.WriteLine("Invalid choice.");
                    continue;
            }
            // if we hit here from case "1" or "2", break out of loop and start the run
            break;
        }

        // Grants the player a starter deck and initializes the player
        if (deck == null)
            deck = starterDeck();
        if (player == null)
            player = new Player("Hero", 30, deck);

        Wallet.Attach(new Wallet(startingGold: 30));

        // Floor loop
        bool fightQueued = true;
        while (!player.IsDead) {
            if (fightQueued) {
                _floor++;
                var enemies = EnemyFactory.BuildRosterScaled(Math.Max(1, _floor), _slimeCount, _slimeHp, _slimeDmg, TimeSpan.FromSeconds(_slimeDelay), Rng.Shared);
                using (var ctx = new CombatContext(player, enemies, _defenseBonus)) {
                    if (inventory.GetRelics().Any(r => r.Kind == RelicKind.StartArmor))
                        ctx.AddArmorToPlayer(3);
                    if (inventory.GetRelics().Any(r => r.Kind == RelicKind.ExtraHp))
                        player.Heal(5);
                    Console.WriteLine($"Ready for floor {_floor}?");
                    Console.ReadLine();

                    await CombatRunner.RunAsync(ctx, inventory);
                }

                var reward = UI.CardRewardScreen.PickOne(TextSpireCS.Model.Cards.CardPool.BasicPool);
                if (reward != default) {
                    deck.AddCard(reward);
                    Console.WriteLine($"Added {reward.Name} to your deck.");
                }

                fightQueued = false;
            } else {
                fightQueued = chooseRoom();
            }
        }

        SaveManager.AppendRun(new RunEntry {
            when = DateTime.Now,
            floor = _floor,
            potionsUsed = 0,
            goldAtEnd = Economy.Wallet.Current?.Gold ?? 0,
            heroClass = Model.Creature.HeroSelection.Current.ToString(),
            notes = ""
        });

        SaveManager.write(_floor, _strikeBonus, _defenseBonus, _slimeDmg, _slimeDelay, _slimeHp, _slimeCount, deck, inventory);

        Console.WriteLine($"You died on floor {_floor}!");
    }

    private static void Pause(string prompt) {
        Console.WriteLine(prompt);
        Console.ReadLine();
    }

    // Main menu logic
    private string MainMenu() {
        Console.WriteLine("""
            
            ===
             1)  Start Game
             2)  Load Save
             3)  Settings
             4)  Exit
             5)  Manual
             6)  Achievements
             7)  Run History
            ===
            
            """);
        Console.Write("Choice: ");
        return Console.ReadLine()?.Trim();
    }

    // settings logic
    private void tweakSettings() {
        _slimeDmg = askInt("Slime damage [Default: 4]  (Int Range: 1-999) : ", 4, 999);
        _slimeDelay = askInt("Attack delay [Default: 2s] (Int Range: 1-30)  : ", 2, 30);
        _slimeHp = askInt("Slime HP     [Default: 20] (Int Range: 1-999) : ", 20, 999);
        _slimeCount = askInt("Slime count  [Default: 2]  (Int Range: 1-8)   : ", 2, 8);
    }

    // Load save logic
    private bool loadSave() {
        if (!SaveManager.exists()) {
            Console.WriteLine("No save found.");
            return false;
        }
        try {
            SaveGame sg = SaveManager.read();
            _floor = sg.floor;
            _strikeBonus = sg.strikeBonus;
            _defenseBonus = sg.defenseBonus;
            _slimeDmg = sg.dmg;
            _slimeDelay = sg.delay;
            _slimeHp = sg.hp;
            _slimeCount = sg.slimeCount;
            deck = new Deck(sg.deck);
            inventory.GetPotions().AddRange(sg.potions);
            inventory.GetWeapons().AddRange(sg.weapons);
            foreach (var r in sg.relics) inventory.AddRelic(r);
            Console.WriteLine($"Save loaded - floor {_floor}");
            return true;
        }
        catch (Exception e) {
            Console.WriteLine("Save corrupt: " + e.Message);
            return false;
        }
    }

    // starter deck
    private static Deck starterDeck() {
        return new Deck(new List<Card> {
                new("Strike", 6),
                new("Strike", 6),
                new("Strike", 6),
                new("Defend", 0),
                new("Defend", 0)});
    }

    // Damage bonus recieved from all current weapons
    private int totalWeaponBonus() {
        return inventory.GetWeapons().Sum(w => w.DamageBonus);
    }

    // Room chooser logic
    private bool chooseRoom() {
        List<Room> options = RoomPicker.ThreeRandomUniform();

        Console.WriteLine("\n== Choose your path ==");
        for (int i = 0; i < options.Count; i++)
            Console.WriteLine($"  {i + 1}) {options[i].ToString()}");

        int pick = askInt("Your choice (1-3): ", 1, 3) - 1;
        Room chosen = options[pick];

        // Autosave before the room mutates state
        try {
            SaveManager.write(_floor, _strikeBonus, _defenseBonus,
                _slimeDmg, _slimeDelay, _slimeHp, _slimeCount,
                deck, inventory);
        }
        catch (Exception e) {
            Console.WriteLine("Could not save: " + e.Message);
        }

        switch (chosen.Type) {
            case RoomType.REST:
                restRoom();
                return false;

            case RoomType.OFFENSE:
                offenseRoom();
                return false;

            case RoomType.DEFENSE:
                defenseRoom();
                return false;

            case RoomType.MONSTER:
                Console.WriteLine("Another fight!");
                return true;

            case RoomType.SHOP:
                UI.ShopScreen.Enter(inventory);
                AchievementTracker.Instance.Unlock(Achievements.BigSpender);
                return false;
            case RoomType.EVENT:
                UI.EventScreen.Enter(player, inventory);
                AchievementTracker.Instance.Unlock(Achievements.FirstEvent);
                return false;
            case RoomType.TREASURE:
                int mult;
                int mut;
                if (inventory.GetRelics().Any(r => r.Kind == RelicKind.GoldBoost)) {
                    mult = 2;
                } else {
                    mult = 1;
                }
                if (TextSpireCS.Model.Misc.MutatorRules.Current == Mutator.Rich) {
                    mut = 2;
                } else {
                    mut = 1;
                }
                int gold = 15 * mult * mut;
                    Economy.Wallet.Current?.Add(gold);
                inventory.AddPotion(new Potion("Treasure Potion", 10));
                Console.WriteLine($"You found a chest: +{gold} gold and a potion!");
                return false;

            case RoomType.MYSTERY:
                var roll = Rng.Next(0, 3);
                if (roll == 0) restRoom();
                else if (roll == 1) offenseRoom();
                else defenseRoom();
                Console.WriteLine("(You took a mysterious detour...)");
                return false;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    // Room logic
    private void restRoom() {
        int missing = player.MaxHp - player.Hp;
        int heal = (int)(missing * 0.75);
        player.Heal(heal);
        inventory.AddPotion(new Potion("Rest Potion", -1));
        Console.WriteLine($"You rest and heal {heal} HP (HP {player.Hp})");
    }
    private void offenseRoom() {
        _strikeBonus++;
        Weapon w = new("Blade +" + inventory.GetWeapons().Count, 1);
        inventory.AddWeapon(w);
        deck.AddCard(new Card("Strike", 6));
        Console.WriteLine($"Offense upgrade! Strike bonus {_strikeBonus}, weapon bonus +{totalWeaponBonus()}");
    }
    private void defenseRoom() {
        _defenseBonus++;
        deck.AddCard(new Card("Defend", 0));
        Console.WriteLine($"Defense upgrade! Defend bonus {_defenseBonus}");
    }

    // Utility method to get an int from the user
    private int askInt(string prompt, int defVal, int max) {
        while (true) {
            Console.Write(prompt);
            string? line = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(line))
                return defVal;
            try {
                int n = int.Parse(line);
                if (n < 1 || n > max) throw new FormatException();
                return n;
            }
            catch (FormatException) {
                Console.WriteLine($"Enter 1-{max} or blank for {defVal}.");
            }
        }
    }
}