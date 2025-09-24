using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSpireCS.Model.Creature;
using TextSpireCS.Model.Item;

namespace TextSpireCS.UI;

public static class EventScreen {
    private static readonly Random _rng = new();

    public static void Enter(Player player, Inventory inv) {
        Console.WriteLine("\nYou stumble upon a cozy rock. Meditate? (y/n)");
        var k = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();
        if (k == "y") {
            int roll = _rng.Next(100);
            if (roll < 40) {
                inv.AddPotion(new Potion("Rock potion", -1));
                Console.WriteLine("You find a potion near the rock. (+1 potion)");
            } else if (roll < 80) {
                int heal = Math.Max(1, (player.MaxHp - player.Hp) / 3);
                player.Heal(heal);
                Console.WriteLine($"You feel better. (+{heal} HP)");
            } else {
                Console.WriteLine("You feel uneasy. Nothing happens.");
            }
        } else {
            Console.WriteLine("You walk past the rock.");
        }
    }
}