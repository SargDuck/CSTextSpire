using System.Text.Json;
using TextSpireCS.Model.Item;
using Xunit;

namespace TextSpireCS.Tests;

public class WeaponTests {
    [Fact]
    public void ConstrSetsNameAndBonus() {
        var w = new Weapon("Sword", 3);

        Assert.Equal("Sword", w.Name);
        Assert.Equal(3, w.DamageBonus);
        Assert.Equal(3, w.Damage.Total);
        Assert.Equal(3, w.Hit());
    }

    [Fact]
    public void ParameterlessConstrAllowsPropertySettersAndComputedDamageTracks() {
        var w = new Weapon();
        w.Name = "Axe";
        w.DamageBonus = 5;

        Assert.Equal("Axe", w.Name);
        Assert.Equal(5, w.DamageBonus);
        Assert.Equal(5, (int)w.Damage);
        Assert.Equal(5, w.Hit());
    }

    [Fact]
    public void DamageSumsBaseAndBonusAndSupportsAddition() {
        var d1 = new Damage(b: 2, bonus: 1);
        var d2 = new Damage(b: 1, bonus: 3);

        var d3 = d1 + d2;

        Assert.Equal(3, d3.Base);
        Assert.Equal(4, d3.Bonus);
        Assert.Equal(7, d3.Total);
        Assert.Equal(7, (int)d3);
    }

    [Fact]
    public void ChangingBonusUpdatesComputedDamage() {
        var w = new Weapon("Dagger", 1);
        Assert.Equal(1, w.Hit());

        w.DamageBonus = 6;

        Assert.Equal(6, w.Damage.Total);
        Assert.Equal(6, w.Hit());
    }
}