using System.Text;
using TextSpireCS.Model.Card;
using TextSpireCS.Model.Creature;
using TextSpireCS.Model.Item;
using TextSpireCS.UI;
using TextSpireCS.Util;
using TextSpireCS.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace TextSpireCS.Engine;

public static class CombatRunner {

    // Spawns one task per enemy.
    public static async Task RunAsync(CombatContext ctx, Inventory inv) {
        var enemyTasks = new List<Task>();
        for (int i = 0; i < ctx.Enemies.Count; i++) {
            var e = ctx.Enemies[i];
            // Each task is staggered by 50ms to avoid synchronized strikes.
            var delayMs = i * 50;
            enemyTasks.Add(SpawnEnemyAsync(e, ctx, delayMs));
        }

        // Launches a keyboard thread that: 
        //  reads keys as you type, 
        //  builds a buffer,
        //  on Enter, parses a command and dispatches it,
        //  exists automatically when ctx.CombatEnded == true.
        var inputTask = Task.Run(() => HandleInput(ctx, inv));

        // Main thread polls every 100ms until ctx.SignalCombatEnd() is called by:
        //  last enemy dying (from play logic), 
        //  the player dying (enemy tasks detect this), 
        //  the user typing quit.
        while (!ctx.CombatEnded) {
            ThreadUtils.SleepQuietly(100);
        }

        // After the end is signaled, 
        //  each enemy task exists because ctx.Token is cancelled,
        //  input thread exits because the loop condition flips,
        //  then join all tasks and print a finish line.
        try { await Task.WhenAll(enemyTasks); } catch { // ignore
        }
        try { await inputTask; }
        catch { // ignore 
        }
        Console.WriteLine("\n*** Combat finished ***");
    }

    // Awaits a stagger delay cancellable by ctx.Token.
    // Starts the enemy loop (your Enemy.RunAsync), which wakes up every interval and applies
    // damage to the player under ctx.Lock.
    // Any cancellation (combat end) comes as OperationCanceledException which is treated like a normal shutdown.
    private static async Task SpawnEnemyAsync(Enemy e, CombatContext ctx, int initialDelayMs) {
        if (initialDelayMs > 0)
            try { await Task.Delay(initialDelayMs, ctx.Token); } catch (OperationCanceledException) { return; }

        try { await e.RunAsync(ctx); }
        catch (OperationCanceledException) { // normal  
        }
    }


    // Console.KeyAvailable polls with a 25ms sleep time to keep CPU usage low.
    // Echoes typed characters, support backspace, and ignores other control keys.
    // On Enter, it parses via CommandParser (Play N, use, status, quit, help) and dispatches.
    // When combat ends, the while condition flips and the input thread exits.
    private static void HandleInput(CombatContext ctx, Inventory inv) {
        ShowHelp();
        Console.Write("> ");
        var buf = new StringBuilder();
        while (!ctx.CombatEnded) {
            if (!Console.KeyAvailable) {
                ThreadUtils.SleepQuietly(25);
                continue;
            }
            var key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Enter) {
                Console.WriteLine();
                var line = buf.ToString();
                buf.Clear();
                var (cmd, arg, ok) = CommandParser.Parse(line);
                if (!ok) { Console.WriteLine("Typo!"); continue; }
                Dispatch(cmd, arg, ctx, inv);
                if (!ctx.CombatEnded)
                    Console.Write("> ");
                continue;
            }
            if (key.Key == ConsoleKey.Backspace) {
                if (buf.Length > 0) {
                    buf.Length--;
                    Console.Write("\b \b");
                }
                continue;
            }
            if (char.IsControl(key.KeyChar)) continue;
            buf.Append(key.KeyChar);
            Console.Write(key.KeyChar);
        }
    }

    // Routes a parsed command to its appropriate handler.
    private static void Dispatch(string cmd, int arg, CombatContext ctx, Inventory inv) {
        switch (cmd) {
            case "play": PlayCard(arg, ctx); break;
            case "use": UsePotion(ctx, inv); break;
            case "status": StatusPrinter.Print(ctx, inv); break;
            case "quit": ctx.SignalCombatEnd(); break;
            case "help": ShowHelp(); break;
        }
    }


    // Draws and plays a card on a target enemy (1-based index), then discards.
    // Ends combat if that was the last living enemy.
    // All mutations ((draw, target validation, damage, discard, end-check)
    // happens inside ctx.Lock to keep state consistent with enemy threads.
    // Printing happens after the lock is released.
    private static void PlayCard(int idx1Based, CombatContext ctx) {
        Card? played = null;
        Enemy? target = null;
        int targetHpAfter = 0;

        lock (ctx.Lock) {
            if (ctx.Enemies.All(e => e.Hp <= 0)) {
                Console.WriteLine("No Targets!");
                ctx.SignalCombatEnd(); // No target remain.
                return;
            }

            played = ctx.Player.Deck.Draw(); // draw.
            if (played is null) {
                Console.WriteLine("Draw pile empty!");
                return;
            }

            if (idx1Based < 1 || idx1Based > ctx.Enemies.Count) {
                Console.WriteLine($"Choose enemy 1-{ctx.Enemies.Count}");
                return;
            }
            target = ctx.Enemies[idx1Based - 1];
            if (target.Hp <= 0) {
                Console.WriteLine("Already down!");
                return;
            }

            // effect
            target.TakeDamage(played.Dmg);
            targetHpAfter = target.Hp;

            // discard after effect
            ctx.Player.Deck.Discard(played);

            // end if that was the last living enemy
            if (target.Hp <= 0 && ctx.Enemies.All(e => e.Hp <= 0))
                ctx.SignalCombatEnd();
        }

        // print outside lock to minimize time spent inside critical section
        Console.WriteLine($"You played {played!.Name}! {target!.Name} HP: {targetHpAfter}");
    }


    // Uses the first potion in the inventory to heal the player.
    private static void UsePotion(CombatContext ctx, Inventory inv) {
        string? pname = null;
        int healed = 0;
        int hpAfter = 0;
        lock (ctx.Lock) {
            if (inv.GetPotions().Count == 0)
                return; // print after lock

            var p = inv.GetPotions()[0];
            inv.GetPotions().RemoveAt(0);

            int missing = ctx.Player.MaxHp - ctx.Player.Hp;
            healed = (p.Potency == -1) ? missing / 2 : p.Potency;

            ctx.Player.Heal(healed);
            hpAfter = ctx.Player.Hp;
            pname = p.Name;
        }

        if (pname is null) { Console.WriteLine("No potions left!"); return; }
        Console.WriteLine($"You drink {pname}! You heal {healed}! (HP: {hpAfter})");
    }
    

    // Status routes to StatusPrinter.Print(ctx, inv).
    // help prints the mini command list.
    // quit simply calls ctx.SignalCombatEnd().
    private static void ShowHelp() {
        Console.WriteLine("""
            play N   – use drawn card on enemy N
            use      – drink first potion
            quit     – end combat
            status   – show HP/deck/inventory
            help     – this list
            """);
    }
    private static void PrintStatus(CombatContext ctx, Inventory inv) {
        var sb = new StringBuilder();
        sb.AppendLine($"HP: {ctx.Player.Hp}/{ctx.Player.MaxHp}");
        for (int i = 0; i < ctx.Enemies.Count; i++) {
            var e = ctx.Enemies[i];
            sb.AppendLine($"[{i + 1}] {e.Name}: {e.Hp} HP, hits for {e.Damage}");
        }
        sb.AppendLine($"Potions: {inv.GetPotions().Count}, Weapons: {inv.GetWeapons().Count}");
        Console.WriteLine(sb.ToString());
    }
}
