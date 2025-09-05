using TextSpireCS.Engine;
using TextSpireCS.Model.Card;
using TextSpireCS.Model.Creature;
using TextSpireCS.Model.Item;
using TextSpireCS.Model.World;
using TextSpireCS.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextSpireCS.UI;
using TextSpireCS.Persist;

namespace TextSpireCS;

public class Program
{
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
    public static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var game = new Program();
        await game.RunAsync();
    }

    private async Task RunAsync() { 
        
        // Prints a banner and a main menu.
        BannerPrinter.Print();
        string choice = MainMenu();
        switch (choice)
        {
            // Lets the player load a save or tweak settings before starting the game.
            case "4":
                Console.WriteLine("Bye!");
                return;
            case "2":
                if (!loadSave())
                {
                    Console.WriteLine("No save found.");
                    return;
                }
                Console.WriteLine("Load successful.");
                break;
            case "3":
                tweakSettings();
                break;
        }

        // Grants the player a starter deck and initializes the player
        if (deck == null)
        {
            deck = starterDeck();
        }
        player = new Player("Hero", 30, deck);

        // Floor loop
        bool fightQueued = true;
        while (!player.IsDead)
        {
            if (fightQueued)
            {
                _floor++;
                var enemies = EnemyFactory.BuildScaledFromBase(Math.Max(1, _floor), _slimeCount,_slimeHp,_slimeDmg,TimeSpan.FromSeconds(_slimeDelay),Rng.Shared);
                using (var ctx = new CombatContext(player, enemies)) {
                    Console.WriteLine($"Ready for floor {_floor}?");
                    Console.ReadLine();
                    
                    await CombatRunner.RunAsync(ctx, inventory);
                }
                
                fightQueued = false;
            } else {
                fightQueued = chooseRoom();
            }
        }
        Console.WriteLine($"You died on floor {_floor}!");
    }

    // Main menu logic
    private string MainMenu()
    {
        Console.WriteLine("""
            
            ===
             1)  Start Game
             2)  Load Save
             3)  Settings
             4)  Exit
            ===
            
            """);
        Console.Write("Choice: ");
        return Console.ReadLine()?.Trim();
    }

    // settings logic
    private void tweakSettings()
    {
        _slimeDmg   = askInt("Slime damage [Default: 4]  (Int Range: 1-999) : ", 4,  999);
        _slimeDelay = askInt("Attack delay [Default: 2s] (Int Range: 1-30)  : ",  2,  30);
        _slimeHp    = askInt("Slime HP     [Default: 20] (Int Range: 1-999) : ",  20, 999);
        _slimeCount = askInt("Slime count  [Default: 2]  (Int Range: 1-8)   : ",  2,  8);
    }

    // Load save logic
    private bool loadSave()
    {
        if (!SaveManager.exists())
        {
            Console.WriteLine("No save found.");
            return false;
        }
        try
        {
            SaveGame sg  = SaveManager.read();
            _floor        = sg.floor;
            _strikeBonus  = sg.strikeBonus;
            _defenseBonus = sg.defenseBonus;
            _slimeDmg     = sg.dmg;
            _slimeDelay   = sg.delay;
            _slimeHp      = sg.hp;
            _slimeCount   = sg.slimeCount;
            deck         = new Deck(sg.deck);
            inventory.GetPotions().AddRange(sg.potions);
            inventory.GetWeapons().AddRange(sg.weapons);
            Console.WriteLine($"Save loaded – floor {_floor}");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine("Save corrupt: " + e.Message);
            return false;
        }
    }

    // starter deck
    private static Deck starterDeck()
    {
        return new Deck(new List<Card> {
                new("Strike", 6),
                new("Strike", 6),
                new("Strike", 6),
                new("Defend", 0),
                new("Defend", 0)});
    }

    // Damage bonus recieved from all current weapons
    private int totalWeaponBonus()
    {
        return inventory.GetWeapons().Sum(w => w.DamageBonus);
    }

    // Room chooser logic
    private bool chooseRoom()
    {
        List<Room> options = RoomPicker.ThreeRandomUniform();

        Console.WriteLine("\n== Choose your path ==");
        for (int i = 0; i < options.Count; i++)
            Console.WriteLine($"  {i+1}) {options[i].ToString()}");

        int pick = askInt("Your choice (1-3): ", 1, 3) - 1;
        Room chosen = options[pick];

        // Autosave before the room mutates state
        try
        {
            SaveManager.write(_floor, _strikeBonus, _defenseBonus,
                _slimeDmg, _slimeDelay, _slimeHp, _slimeCount,
                deck, inventory);
        }
        catch (Exception e)
        {
            Console.WriteLine("Could not save: " + e.Message);
        }

        switch (chosen.Type)
        {
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

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    // Room logic
    private void restRoom()
    {
        int missing = player.MaxHp - player.Hp;
        int heal = (int)(missing * 0.75);
        player.Heal(heal);
        inventory.AddPotion(new Potion("Rest Potion", -1));
        Console.WriteLine($"You rest and heal {heal} HP (HP {player.Hp})");
    }
    private void offenseRoom()
    {
        _strikeBonus++;
        Weapon w = new("Blade +" + inventory.GetWeapons().Count, 1);
        inventory.AddWeapon(w);
        deck.AddCard(new Card("Strike", 6));
        Console.WriteLine($"Offense upgrade! Strike bonus {_strikeBonus}, weapon bonus +{totalWeaponBonus()}");
    }
    private void defenseRoom()
    {
        _defenseBonus++;
        deck.AddCard(new Card("Defend", 0));
        Console.WriteLine($"Defense upgrade! Defend bonus {_defenseBonus}");
    }

    // Utility method to get an int from the user
    private int askInt(string prompt, int defVal, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            string? line = Console.ReadLine()?.Trim();
            if (string.IsNullOrWhiteSpace(line))
                return defVal;
            try
            {
                int n = int.Parse(line);
                if (n < 1 || n > max) throw new FormatException();
                return n;
            }
            catch (FormatException)
            {
                Console.WriteLine($"Enter 1-{max} or blank for {defVal}.");
            }
        }
    }

    // Unused since .ToString() added to Room.cs
    //private static string pretty(Room r) =>
    //    r.Type switch
    //    {
    //        RoomType.REST => "Rest",
    //        RoomType.MONSTER => "Monster",
    //        RoomType.OFFENSE => "Offense Upgrade",
    //        RoomType.DEFENSE => "Defense Upgrade",
    //        _ => throw new ArgumentOutOfRangeException()
    //    };
}
