using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSpireCS.Model.Creature;

namespace TextSpireCS.UI;

public static class HeroSelectScreen {
    public static HeroClass Choose() {
        Console.WriteLine("""
                === Choose your Hero ===
                 1) Warrior  (high HP)
                 2) Rogue    (more Strikes)
                 3) Mage     (more Defends)
                """);
        Console.Write("Pick: ");
        var s = Console.ReadLine();
        return s switch {
            "2" => HeroClass.Rogue,
            "3" => HeroClass.Mage,
             _ => HeroClass.Warrior,
        };
    }
}