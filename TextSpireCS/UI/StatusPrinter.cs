using System;
using System.Linq;
using TextSpireCS.Engine;
using TextSpireCS.Model.Creature;
using TextSpireCS.Model.Item;

namespace TextSpireCS.UI {

    // Provideds a snapshot of the player's current state:
    //  Player HP ( Hp / MaxHp ).
    //  Deck size ( Deck.Size ).
    //  Potions count.
    //  Weapons count.
    //  Sum of weapon damage bonuses.
    public static class StatusPrinter {
        public static void Print(CombatContext ctx, Inventory inv) {

            if (ctx is null) throw new ArgumentNullException(nameof(ctx));
            if (inv is null) throw new ArgumentNullException(nameof(inv));

            Player p = ctx.Player;

            Console.WriteLine($"HP {p.Hp} / {p.MaxHp}");

            int deckSize = p.Deck.Size;
            Console.WriteLine($"Deck size : {deckSize}");

            int weaponBonus = inv.GetWeapons().Sum(w => w.DamageBonus);

            Console.WriteLine($"Potions   : {inv.GetPotions().Count}");
            Console.WriteLine($"Weapons   : {inv.GetWeapons().Count}  (bonus +{weaponBonus})");
        }
    }
}
