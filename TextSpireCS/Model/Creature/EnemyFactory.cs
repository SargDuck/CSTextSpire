using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace TextSpireCS.Model.Creature;

public static class EnemyFactory
{
    // Creates the same enemy count number of times.
    // The rng var is passed but is unused.
    public static List<Enemy> BuildScaledFromBase(int floor, int count, int baseHp, int baseDmg, TimeSpan interval, Random rng) {

        var list = new List<Enemy>();
        for (int i = 0; i < count; i++) {
            int hp = Scale(baseHp, floor);
            int dmg = Scale(baseDmg, floor);

            list.Add(new Enemy($"Slime {i+1}", hp, dmg, interval));
        }
        return list;
    }


    public static List<Enemy> BuildRosterScaled(
            int floor, int count, int baseHp, int baseDmg, TimeSpan interval, Random rng) {
        var list = new List<Enemy>();
        for (int i = 0; i < count; i++) {
            var kind = rng.Next(4);
            switch (kind) {
                case 0: { // Slime (base)
                        int hp = baseHp + floor * 3;
                        int dmg = baseDmg + floor * 3;
                        list.Add(new Enemy($"Slime {i + 1}", hp, dmg, interval));
                        break;
                    }
                case 1: { // Bat (faster, weaker)
                        int hp = baseHp - 2 + floor * 2;
                        int dmg = baseDmg - 1 + floor * 2;
                        var fast = TimeSpan.FromMilliseconds(Math.Max(400, interval.TotalMilliseconds * 0.7));
                        list.Add(new Enemy($"Bat {i + 1}", hp, dmg, fast));
                        break;
                    }
                case 2: { // Goblin (average)
                        int hp = baseHp + 1 + floor * 3;
                        int dmg = baseDmg + 1 + floor * 2;
                        list.Add(new Enemy($"Goblin {i + 1}", hp, dmg, interval));
                        break;
                    }
                default: { // Bear (slow, heavy hit)
                        int hp = baseHp + floor * 3;
                        int dmg = baseDmg + 2 + floor * 4;
                        var slow = TimeSpan.FromMilliseconds(interval.TotalMilliseconds * 1.3);
                        list.Add(new Enemy($"Bear {i + 1}", hp, dmg, slow));
                        break;
                    }
            }
        }
        return list;
    }

    // The scaling formulas is simply base + floor * scalar (defaulted to 3).
    private static int Scale(int b, int floor, int scalar = 3) => b + floor * scalar;
}