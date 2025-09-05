using System.Text.Json;
using TextSpireCS.Model.Card;
using TextSpireCS.Model.Item;
using TextSpireCS.Persist;
using Xunit;

public class SaveGameTests {
    [Fact]
    public void SaveGameSerializesAndDeserializesTest() {
        var sg = new SaveGame {
            floor = 2,
            strikeBonus = 1,
            defenseBonus = 0,
            dmg = 4,
            delay = 2,
            hp = 20,
            slimeCount = 3,
            deck = new() { new Card("Strike", 6) },
            potions = new() { new Potion("Heal", 10) },
            weapons = new() { new Weapon("Sword", 2) }
        };

        var json = JsonSerializer.Serialize(sg);
        var copy = JsonSerializer.Deserialize<SaveGame>(json);

        Assert.Equal(sg.floor, copy!.floor);
        Assert.Single(copy.deck);
        Assert.Equal("Strike", copy.deck[0].Name);
    }
}
