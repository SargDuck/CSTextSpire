using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSpireCS.Engine;
using TextSpireCS.Model.Cards;
using TextSpireCS.Model.Creature;

public class StatusEffectsTests {
    [Fact]
    public void ArmorAbsorbsDamageBeforeHp() {
        var p = new Player("Hero", 20, new Deck(new[] { new Card("Defend", 0) }));
        var e = new Enemy("Slime", 10, 5, TimeSpan.FromSeconds(1));
        using var ctx = new CombatContext(p, new List<Enemy> { e }, defenseBonus: 0);

        ctx.AddArmorToPlayer(6); // grant armor
        int dmg = ctx.AbsorbDamageToPlayer(5);
        Assert.Equal(0, dmg); // fully absorbed
    }

    [Fact]
    public void VulnerableIncreasesDamage() {
        var p = new Player("Hero", 20, new Deck(new[] { new Card("Strike", 6) }));
        var e = new Enemy("Slime", 20, 5, TimeSpan.FromSeconds(1));
        using var ctx = new CombatContext(p, new List<Enemy> { e }, 0);

        ctx.ApplyVulnerable(e, 1);
        int mod = ctx.ModifyOutgoingPlayerDamage(10, e);
        Assert.True(mod >= 15); // 1.5x
    }

    [Fact]
    public void WeakReducesEnemyDamage() {
        var p = new Player("Hero", 20, new Deck(new[] { new Card("Strike", 6) }));
        var e = new Enemy("Slime", 20, 8, TimeSpan.FromSeconds(1));
        using var ctx = new CombatContext(p, new List<Enemy> { e }, 0);

        ctx.ApplyWeak(e, 1);
        int mod = ctx.ModifyOutgoingEnemyDamage(e, 8);
        Assert.True(mod <= 6); // 0.75x
    }
}