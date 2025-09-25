using System.Threading;
using System;
using System.Collections.Generic;
using TextSpireCS.Model.Creature;

namespace TextSpireCS.Engine;


// Shared blackboard where all combat participants read/write to while a fight is running. It is safe from race conditions
// and partial views. The class has locks for safe updates, cancellation token to stop background loops, 
// combat ended flag that's safe for al threads to read, and a read-only snapshot of the enemies list thats unmodifiable.
public sealed class CombatContext : IDisposable
{
    private readonly object _lock = new();
    
    // Handle to broadcast a cancellation to all loops (enemy attack timers, delays, etc.).
    private readonly CancellationTokenSource _cts = new();

    public object Lock => _lock;

    // Shared references for everyone
    public Player Player { get; }
    public IReadOnlyList<Enemy> Enemies { get; }

    public int DefenseBonus { get; }
    private int _playerArmor;
    private readonly Dictionary<Enemy, int> _weak = new();
    private readonly Dictionary<Enemy, int> _vuln = new();

    // Null checks for quick fails. Immutable enemies list since this class doesn't modify enemies.
    public CombatContext(Player player, IReadOnlyList<Enemy> enemies, int defenseBonus)
    {
        Player = player ?? throw new ArgumentNullException(nameof(player));
        Enemies = (enemies ?? throw new ArgumentNullException(nameof(enemies))) is List<Enemy> list ? list.AsReadOnly() : new List<Enemy>(enemies).AsReadOnly();
        DefenseBonus = defenseBonus;

        // Aarmor is temporary per floor/combat
        Player.ResetArmor();
    }

    // Token passed to delays/timers/loops. When Cancel() happens, they stop.
    public CancellationToken Token => _cts.Token;

    // Volatile ensures the read sees the latest write done on another thread. 
    public bool CombatEnded => _cts.IsCancellationRequested;

    // Calls Cancel() to stop loops.
    public void SignalCombatEnd() {
        _cts.Cancel();
    }

    public int AbsorbDamageToPlayer(int raw) {
        lock (_lock) {
            int dmg = Math.Max(0, raw);
            if (_playerArmor > 0) {
                int absorbed = Math.Min(_playerArmor, dmg);
                _playerArmor -= absorbed;
                dmg -= absorbed;
                if (absorbed > 0) Console.WriteLine($"(Armor absorbed {absorbed}, armor left {_playerArmor})");
            }
            return dmg;
        }
    }

    public void AddArmorToPlayer(int amount) {
        lock (_lock) _playerArmor = Math.Max(0, _playerArmor + Math.Max(0, amount));
        Console.WriteLine($"(Armor +{amount}, total armor: {_playerArmor})");
    }

    public void ApplyWeak(Enemy e, int stacks) {
        lock (_lock) { _weak[e] = (_weak.TryGetValue(e, out var n) ? n : 0) + stacks; }
        Console.WriteLine($"{e.Name} is weakened ({_weak[e]})!");
    }
    public void ApplyVulnerable(Enemy e, int stacks) {
        lock (_lock) { _vuln[e] = (_vuln.TryGetValue(e, out var n) ? n : 0) + stacks; }
        Console.WriteLine($"{e.Name} is vulnerable ({_vuln[e]})!");
    }

    // Modifies damage outgoing from the player to an enemy
    public int ModifyOutgoingPlayerDamage(int baseDmg, Enemy target) {
        lock (_lock) {
            int dmg = Math.Max(0, baseDmg);
            if (_vuln.TryGetValue(target, out var stacks) && stacks > 0)
                dmg = (int)Math.Round(dmg * 1.5); // +50%
            return dmg;
        }
    }

    // Modifies damage outgoing from an enemy to the player
    public int ModifyOutgoingEnemyDamage(Enemy e, int baseDmg) {
        lock (_lock) {
            int dmg = Math.Max(0, baseDmg);
            if (_weak.TryGetValue(e, out var stacks) && stacks > 0)
                dmg = (int)Math.Round(dmg * 0.75); // -25%
            return dmg;
        }
    }

    // Cleans up the token.
    public void Dispose() => _cts.Dispose();
}
