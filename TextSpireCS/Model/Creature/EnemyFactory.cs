using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace TextSpireCS.Model.Creature;

// Provides a static factory that spawns a list of enemies with Hp and damage that
// scales with the floor.
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

    // The scaling formulas is simply base + floor * scalar (defaulted to 3).
    private static int Scale(int b, int floor, int scalar = 3) => b + floor * scalar;
}