using System;
using System.Collections.Generic;
using System.Linq;
using TextSpireCS.Model.World;

namespace TextSpireCS.Util;

// Static class that picks 3 rooms from RoomType at random.
public static class RoomPicker {
    private static readonly Random _rng = new();

    // Three random rooms with uniform randomness
    public static List<Room> ThreeRandomUniform() {
        var types = new[] {
                RoomType.REST, RoomType.OFFENSE, RoomType.DEFENSE, RoomType.MONSTER,
                RoomType.SHOP, RoomType.EVENT, RoomType.TREASURE, RoomType.MYSTERY
            };
        var list = new List<Room>();
        for (int i = 0; i < 3; i++) {
            var t = types[_rng.Next(types.Length)];
            list.Add(new Room(t));
        }
        return list;
    }

    // Three random unique rooms
    public static List<Room> ThreeUnique() {
        var types = Enum.GetValues<RoomType>().ToList();
        types = types.OrderBy(_ => _rng.Next()).ToList();
        return types.Take(3).Select(t => new Room(t)).ToList();
    }

    // Three random rooms with a weighed distribution.
    private static readonly (RoomType Type, int Weight)[] Weighted = {
    (RoomType.MONSTER, 30),
    (RoomType.REST,    10),
    (RoomType.OFFENSE, 10),
    (RoomType.DEFENSE, 10),
    (RoomType.EVENT, 10),
    (RoomType.SHOP, 10),
    (RoomType.TREASURE, 10),
    (RoomType.MYSTERY, 10)
};

    public static Room PickWeighted() {
        int total = Weighted.Sum(w => w.Weight);
        int roll = _rng.Next(total);
        foreach (var (type, weight) in Weighted) {
            if (roll < weight) return new Room(type);
            roll -= weight;
        }
        throw new InvalidOperationException("Weighted pick failed");
    }
}