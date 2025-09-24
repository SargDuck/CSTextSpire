using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSpireCS.Economy;
using TextSpireCS.Model.Item;

namespace TextSpireCS.UI;

public static class ShopScreen {
    private sealed record Offer(string Label, int Price, Action Buy);

    public static void Enter(Inventory inv) {
        var wallet = Wallet.Current ?? new Wallet();
        Wallet.Attach(wallet);

        var offers = new List<Offer> {
                new("Potion +10 HP", 15, () => inv.AddPotion(new Potion("Heal", 10))),
                new("Potion 50% heal", 25, () => inv.AddPotion(new Potion("Big Heal", -1))),
                new("Weapon +1", 30, () => inv.AddWeapon(new Weapon("Dagger +1", 1))),
                new("Weapon +2", 50, () => inv.AddWeapon(new Weapon("Sword +2", 2)))
            };

        while (true) {
            Console.WriteLine($"\n=== Shop (Gold: {wallet.Gold}) ===");
            for (int i = 0; i < offers.Count; i++)
                Console.WriteLine($" {i + 1}) {offers[i].Label}  — {offers[i].Price}g");
            Console.WriteLine(" 0) Leave");
            Console.Write("Your choice: ");
            var pick = Console.ReadLine();
            if (pick == "0" || string.IsNullOrWhiteSpace(pick)) return;
            if (int.TryParse(pick, out var n) && n >= 1 && n <= offers.Count) {
                var o = offers[n - 1];
                if (!wallet.Spend(o.Price)) { Console.WriteLine("Not enough gold!"); continue; }
                o.Buy();
                Console.WriteLine($"Purchased {o.Label}.");
            }
        }
    }
}