using TextSpireCS.Model.Item;
using Xunit;

namespace TextSpireCS.Tests;

public class PotionTests {
    [Fact]
    public void ConstructorSetsProperties() {
        var p = new Potion("Heal", 10);
        Assert.Equal("Heal", p.Name);
        Assert.Equal(10, p.Potency);
    }

    [Fact]
    public void DefaultConstructorAllowsDeserialization() {
        var p = new Potion();
        p.Name = "Big Heal";
        p.Potency = 50;

        Assert.Equal("Big Heal", p.Name);
        Assert.Equal(50, p.Potency);
    }

    [Fact]
    public void PotionCanHaveSpecialPotencyValue() {
        var p = new Potion("Rest Potion", -1);

        Assert.Equal(-1, p.Potency);
    }

    [Fact]
    public void MultiplePotionsCanCoexist() {
        var p1 = new Potion("Heal", 10);
        var p2 = new Potion("Big Heal", 50);

        Assert.NotEqual(p1.Name, p2.Name);
        Assert.NotEqual(p1.Potency, p2.Potency);
    }
}
