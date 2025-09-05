namespace TextSpireCS.Model.World;

// Closed set of values. Each case does something does the gameplay loop throught the chooseRoom() function.
public enum RoomType {
    REST,
    OFFENSE,
    DEFENSE,
    MONSTER
}

// Wrapper object around RoomType.
public sealed class Room {
    public RoomType Type { get; }

    public Room(RoomType type) {
        Type = type;
    }
    public override string ToString() => Type switch {
        RoomType.REST => "Rest Room",
        RoomType.OFFENSE => "Offense Room",
        RoomType.DEFENSE => "Defense Room",
        RoomType.MONSTER => "Monster Encounter",
        _ => Type.ToString()
    };
}