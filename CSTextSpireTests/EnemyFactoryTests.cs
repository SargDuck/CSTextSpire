using System;
using System.Linq;
using TextSpireCS.Model.Creature;
using Xunit;

namespace TextSpireCS.Tests;

public class EnemyFactoryScalingTests {
    [Fact]
    public void BuildScaledFromBasePreservesCountAndIntervalAndNamesAreSequential() {
        int floor = 3;
        int count = 3;
        int baseHp = 10;
        int baseDmg = 4;
        var interval = TimeSpan.FromSeconds(2);
        var rng = new Random(123);
        var mobs = EnemyFactory.BuildScaledFromBase(floor, count, baseHp, baseDmg, interval, rng);

        // count preserved
        Assert.Equal(count, mobs.Count);

        // interval preserved
        Assert.All(mobs, e => Assert.Equal(interval, e.Interval));

        // names: "Slime 1", "Slime 2", "Slime 3"
        for (int i = 0; i < count; i++)
            Assert.Equal($"Slime {i+1}", mobs[i].Name);

        // unique names
        Assert.Equal(count, mobs.Select(e => e.Name).Distinct().Count());
    }

    [Theory]
    [InlineData(0, 10, 4, 10, 4)]   // +0
    [InlineData(1, 10, 4, 13, 7)]   // +3*1
    [InlineData(5, 10, 4, 25, 19)]  // +3*5
    public void BuildScaledFromBaseScalesHpAndDamageByThreePerFloor(int floor, int baseHp, int baseDmg, int expectedHp, int expectedDmg) {
        var rng = new Random(42);
        var mobs = EnemyFactory.BuildScaledFromBase(floor, count: 1, baseHp: baseHp, baseDmg: baseDmg, interval: TimeSpan.FromMilliseconds(500), rng);

        var e = Assert.Single(mobs);
        Assert.Equal(expectedHp, e.Hp);
        Assert.Equal(expectedDmg, e.Damage);
    }

    [Fact]
    public void BuildScaledFromBaseIsIndependentOfRandomParameter() {
        var m1 = EnemyFactory.BuildScaledFromBase(2, 2, 7, 3, TimeSpan.FromMilliseconds(250), new Random(1));
        var m2 = EnemyFactory.BuildScaledFromBase(2, 2, 7, 3, TimeSpan.FromMilliseconds(250), new Random(999));

        Assert.Equal(m1.Select(e => (e.Name, e.Hp, e.Damage, e.Interval)).ToArray(),
                     m2.Select(e => (e.Name, e.Hp, e.Damage, e.Interval)).ToArray());
    }
}
