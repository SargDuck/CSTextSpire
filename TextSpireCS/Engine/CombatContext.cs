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

    // Null checks for quick fails. Immutable enemies list since this class doesn't modify enemies.
    public CombatContext(Player player, IReadOnlyList<Enemy> enemies)
    {
        Player = player ?? throw new ArgumentNullException(nameof(player));
        Enemies = (enemies ?? throw new ArgumentNullException(nameof(enemies))) is List<Enemy> list ? list.AsReadOnly() : new List<Enemy>(enemies).AsReadOnly();
    }

    // Token passed to delays/timers/loops. When Cancel() happens, they stop.
    public CancellationToken Token => _cts.Token;

    // Volatile ensures the read sees the latest write done on another thread. 
    public bool CombatEnded => _cts.IsCancellationRequested;

    // Calls Cancel() to stop loops.
    public void SignalCombatEnd() {
        _cts.Cancel();
    }

    // Cleans up the token.
    public void Dispose() => _cts.Dispose();
}
