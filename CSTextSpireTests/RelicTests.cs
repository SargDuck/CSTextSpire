using TextSpireCS.Model.Item;
using TextSpireCS.Model.Misc;
using Xunit;

public class RelicTests {
    [Fact]
    public void InventoryStoresRelics() {
        var inv = new Inventory();
        inv.AddRelic(new Relic(RelicKind.StartArmor, "Shield"));
        Assert.Single(inv.GetRelics());
        Assert.Equal(RelicKind.StartArmor, inv.GetRelics()[0].Kind);
    }
}