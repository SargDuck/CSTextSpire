using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSpireCS.Model.Misc;

public static class Achievements {
    public const string FirstFloor = "first_floor";
    public const string FiveFloors = "five_floors";
    public const string BigSpender = "big_spender";
    public const string FirstEvent = "first_event";
    public const string NoPotion = "no_potions_run";
}

public sealed class AchievementTracker {
    private static readonly Lazy<AchievementTracker> _lazy =
        new(() => new AchievementTracker());
    public static AchievementTracker Instance => _lazy.Value;

    private readonly HashSet<string> _unlocked = new();

    public bool Unlock(string id) => _unlocked.Add(id);
    public bool IsUnlocked(string id) => _unlocked.Contains(id);

    public List<string> Export() => new(_unlocked);
    public void Import(IEnumerable<string> ids) {
        _unlocked.Clear();
        foreach (var id in ids) _unlocked.Add(id);
    }
}