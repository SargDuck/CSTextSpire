using TextSpireCS.Util;
using TextSpireCS.Model.World;
using Xunit;
using System.Linq;

public class RoomPickerTests {
    [Fact]
    public void ThreeRandomReturnsThreeRooms() {
        var rooms = RoomPicker.ThreeRandomUniform();
        Assert.Equal(3, rooms.Count);
    }

    [Fact]
    public void ThreeRandomReturnsValidRoomTypes() {
        var rooms = RoomPicker.ThreeRandomUniform();
        foreach (var r in rooms) {
            Assert.IsType<Room>(r);
            Assert.IsType<RoomType>(r.Type);
        }
    }

    [Fact]
    public void ThreeRandomProducesDifferentResults() {
        var rooms1 = RoomPicker.ThreeRandomUniform();
        var rooms2 = RoomPicker.ThreeRandomUniform();
        Assert.False(rooms1.SequenceEqual(rooms2),
            "Two picks should not always be identical");
    }
}
