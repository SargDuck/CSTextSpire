using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSpireCS.Model.Cards;
using TextSpireCS.Util;

namespace TextSpireCS.UI;

public static class CardRewardScreen {
    public static Card PickOne(IEnumerable<Card> pool) {
        var list = pool.OrderBy(_ => Rng.Shared.Next(0, 1000000)).Take(3).ToList();
        Console.WriteLine("\nChoose a reward card:");
        for (int i = 0; i < list.Count; i++)
            Console.WriteLine($"  {i + 1}) {list[i].Name} ({list[i].Dmg})");
        Console.Write("Pick 1-3: ");
        var sel = Console.ReadLine();
        if (int.TryParse(sel, out int n) && n >= 1 && n <= list.Count)
            return list[n - 1];
        return default;
    }
}