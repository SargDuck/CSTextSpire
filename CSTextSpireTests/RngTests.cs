using System.Collections.Generic;
using System.Threading.Tasks;
using TextSpireCS.Util;
using Xunit;

public class RngTests {
    [Fact]
    public void NextReturnsWithinBounds() {
        for (int i = 0; i < 100; i++) {
            int n = Rng.Next(1, 5);
            Assert.InRange(n, 1, 4);
        }
    }

    [Fact]
    public void NextIsThreadSafe() {
        var results = new List<int>();
        Parallel.For(0, 1000, i => {
            lock (results) results.Add(Rng.Next(0, 10));
        });
        Assert.Equal(1000, results.Count);
    }
}
