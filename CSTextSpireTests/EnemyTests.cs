using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextSpireCS.Engine;
using TextSpireCS.Model.Cards;
using TextSpireCS.Model.Creature;
using Xunit;

namespace TextSpireCS.Tests;

public class EnemyTests {
    [Fact]
    public async Task EnemyDealsDamageOnIntervalAndStopsOnEnd() {
        var player = new Player("Hero", 50, new Deck(Array.Empty<Card>()));
        var enemy = new Enemy("Slime", hp: 5, damage: 3, interval: TimeSpan.FromMilliseconds(60));
        var ctx = new CombatContext(player, new List<Enemy> { enemy }, 0);

        var task = enemy.RunAsync(ctx);

        // Let 3 ticks happen
        await Task.Delay(210);

        // enemy loop should stop
        ctx.SignalCombatEnd(); 

        await task;

        Assert.InRange(50 - player.Hp, 6, 12); // 2–4 hits
    }
}
