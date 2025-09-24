using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSpireCS.Persist;

namespace TextSpireCS.UI;

public static class RunHistoryScreen {
    public static void Show(SaveGame save) {
        Console.WriteLine("\n=== Run History ===");
        if (save.runs == null || save.runs.Count == 0) {
            Console.WriteLine("(empty)");
        } else {
            foreach (var r in save.runs) {
                Console.WriteLine($"{r.when:g}  Floor {r.floor}  Potions {r.potionsUsed}  Gold {r.goldAtEnd}  [{r.heroClass}]  {r.notes}");
            }
        }
    }
}