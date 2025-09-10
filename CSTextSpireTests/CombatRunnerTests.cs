using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextSpireCS.Engine;
using TextSpireCS.Model.Card;
using TextSpireCS.Model.Creature;
using TextSpireCS.Model.Item;
using Xunit;

namespace TextSpireCS.Tests;

public class CombatRunnerTests {
    [Fact]
    public async Task RunAsyncExitsWhenCombatEndSignaled() {
        var player = new Player("Hero", 10, new Deck(new[] { new Card("Strike", 6) }));
        var enemies = new List<Enemy> { new Enemy("Slime", 5, 1, TimeSpan.FromMilliseconds(80)) };
        var ctx = new CombatContext(player, enemies, 0);
        var inv = new Inventory();
        var run = CombatRunner.RunAsync(ctx, inv);

        // Simulate end after brief time (as if last enemy died)
        await Task.Delay(150);
        ctx.SignalCombatEnd();
        await run;
        Assert.True(ctx.CombatEnded);
    }
}
