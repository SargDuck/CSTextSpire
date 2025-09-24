namespace TextSpireCS.Model.Cards;

// Immutable card with a Name and a damag value.
// Defensive cards are set to 0.

// Unused enum
// public enum CardType { Attack, Defend }
public sealed record Card(string Name, int Dmg);
