using TextSpireCS.Model.Item;
using Xunit;

namespace TextSpireCS.Tests;

public class InventoryTests {
    [Fact]
    public void AddPotionIncreasesPotionCount() {
        var inv = new Inventory();
        var potion = new Potion("Heal", 10);

        inv.AddPotion(potion);

        Assert.Single(inv.GetPotions());
        Assert.Equal("Heal", inv.GetPotions()[0].Name);
    }

    [Fact]
    public void AddWeaponIncreasesWeaponCount() {
        var inv = new Inventory();
        var weapon = new Weapon("Sword", 2);

        inv.AddWeapon(weapon);

        Assert.Single(inv.GetWeapons());
        Assert.Equal("Sword", inv.GetWeapons()[0].Name);
        Assert.Equal(2, inv.GetWeapons()[0].DamageBonus);
    }

    [Fact]
    public void GetPotionsReturnsReferenceToList() {
        var inv = new Inventory();
        var potion = new Potion("Heal", 10);

        inv.AddPotion(potion);
        var potions = inv.GetPotions();

        potions.Clear(); // directly mutates internal list

        Assert.Empty(inv.GetPotions());
    }

    [Fact]
    public void GetWeaponsReturnsReferenceToList() {
        var inv = new Inventory();
        var weapon = new Weapon("Sword", 2);

        inv.AddWeapon(weapon);
        var weapons = inv.GetWeapons();

        weapons.Clear(); // directly mutates internal list

        Assert.Empty(inv.GetWeapons());
    }
}
