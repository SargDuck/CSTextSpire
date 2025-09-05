using TextSpireCS.Model.World;
using Xunit;

public class RoomTests {
    [Fact]
    public void RoomHoldsCorrectType() {
        var r = new Room(RoomType.REST);
        Assert.Equal(RoomType.REST, r.Type);
    }

    [Fact]
    public void RoomToStringIsFriendly() {
        var r = new Room(RoomType.OFFENSE);
        Assert.Equal("Offense Room", r.ToString());
    }
}
