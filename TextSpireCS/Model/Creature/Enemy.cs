using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using TextSpireCS.Engine;

namespace TextSpireCS.Model.Creature;

// Represents a single enemy with a Name, Hp, Damage, and an attack Interval.
// Runs a background attack loop (RunAsync) using PeriodicTimer. Every tick:
// 1. Checks if combat ended or the enemy died.
// 2. Under the combat lock (ctx.Lock), applies damage to the player, and
//    if the player dies, signals combat end.
// 3. Prints the result after releasing the lock (to aboid blocking other threads on I/O). 
// 
// TakeDamage(int) reduces HP to 0 min and flips _dead to stop the loop.
public sealed class Enemy
{

    // Hp mutates, rest is immutable
    public string Name { get; }
    public int Hp { get; private set; }
    public int Damage { get; }
    public TimeSpan Interval { get; }

    // _dead is a simple "stop attacking" flag. Volatile here helps with the visibilty when
    // the player thread writes the flag and the enemy loop reads it. 
    private volatile bool _dead;
    public bool IsDead => _dead;

    public Enemy(string name, int hp, int damage, TimeSpan interval)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        if (hp < 0) throw new ArgumentOutOfRangeException(nameof(hp));
        if (damage < 0) throw new ArgumentOutOfRangeException(nameof(damage));
        if (interval <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(interval));
        Hp = hp;
        Damage = damage;
        Interval = interval;
    }


    // Uses the cancellation token so the timer unblocks immediately when combat ends.
    // State mutation (Player.TakeDamage, IsDead) happens inside the lock to avoid race 
    // conditions with the iunput thread (e.g. playing a card while an enemy hits).
    // The Console.WriteLine happens outside the lock since the console isn't needed
    // for the combat logic.
    public async Task RunAsync(CombatContext ctx)
    {
        var timer = new PeriodicTimer(Interval);
        try
        {
            while (!ctx.CombatEnded && !_dead && ctx.Player.Hp > 0)
            {
                await timer.WaitForNextTickAsync(ctx.Token);
                if (_dead || ctx.CombatEnded) break;
                int hpAfter, armorAfter;
                lock (ctx.Lock)
                    {
                        if (ctx.CombatEnded) return; // another thread ended the fight
                    ctx.Player.TakeDamage(Damage);
                    hpAfter = ctx.Player.Hp;
                    armorAfter = ctx.Player.Armor;
                    if (ctx.Player.IsDead) ctx.SignalCombatEnd();
                     }
                Console.WriteLine($"{Name} hits you for {Damage}. (HP: {hpAfter}, Armor: {armorAfter})");
            }
        }
        catch (OperationCanceledException) { }
        finally {
            timer.Dispose();
        }
    }


    // Makes the enemy stop when the Hp hits 0 or if the enemy has 0 attack.
    public void TakeDamage(int dmg) {
        if (_dead) return;
        if (dmg <= 0) return;
        int newHp = Hp - dmg;
        if (newHp <= 0) {
            Hp = 0;
            _dead = true;
        } else {
            Hp = newHp;
        }
    }
}
