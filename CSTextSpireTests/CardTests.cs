using TextSpireCS.Model.Cards;
using Xunit;

namespace TextSpireCS.Tests;

public class CardTests {
    [Fact]
    public void StrikeHasDamage() {
        var strike = new Card("Strike", 6);
        Assert.Equal(6, strike.Dmg);
        Assert.Equal("Strike", strike.Name);
    }

    [Fact]
    public void BlockZeroDamage() {
        var card = new Card("Block", 0);
        Assert.Equal(0, card.Dmg);
        Assert.Equal("Block", card.Name);
    }

    [Fact]
    public void CardEqualityWorks() {
        var a = new Card("Strike", 6);
        var b = new Card("Strike", 6);
        Assert.Equal(a, b);
    }

    [Fact]
    public void CardToStringIsReadable() {
        var c = new Card("Defend", 0);
        Assert.Contains("Defend", c.ToString());
        Assert.Contains("0", c.ToString());
    }
}
