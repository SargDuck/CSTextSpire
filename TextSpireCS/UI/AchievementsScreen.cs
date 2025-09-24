using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSpireCS.Model.Misc;

namespace TextSpireCS.UI;

public static class AchievementsScreen {
    public static void Show() {
        var a = AchievementTracker.Instance;
        Console.WriteLine("\n=== Achievements ===");
        Print("First Floor", Achievements.FirstFloor);
        Print("Five Floors", Achievements.FiveFloors);
        Print("Big Spender", Achievements.BigSpender);
        Print("First Event", Achievements.FirstEvent);
        Print("No Potions Run", Achievements.NoPotion);

        static void Print(string name, string id) =>
            Console.WriteLine($"[{(AchievementTracker.Instance.IsUnlocked(id) ? "X" : " ")}] {name}");
    }
}