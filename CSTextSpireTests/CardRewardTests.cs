using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextSpireCS.Model.Cards;

public class CardRewardTests {
    [Fact]
    public void PoolContainsNewCards() {
        var names = CardPool.BasicPool.Select(c => c.Name).ToList();
        Assert.Contains("Bash", names);
        Assert.Contains("Cleave", names);
        Assert.Contains("Smash", names);
        Assert.Contains("Reinforce", names);
    }
}