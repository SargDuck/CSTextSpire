using System.Collections.Generic;
using TextSpireCS.Model.Card;
using Xunit;

namespace TextSpireCS.Tests;

public class DeckTests {
    [Fact]
    public void DrawAutoRefillsFromDiscardWhenEmpty() {
        var c1 = new Card("A", 1);
        var c2 = new Card("B", 1);
        var deck = new Deck(new[] { c1 });

        // Draw first card (draw pile now empty)
        var d1 = deck.Draw();
        Assert.Equal(c1, d1);

        // Discard played card
        deck.Discard(c1);

        // Next draw should auto refill from discard and not be null
        var d2 = deck.Draw();
        Assert.NotNull(d2);
    }

    [Fact]
    public void SizeReturnsDrawPlusDiscard() {
        var deck = new Deck(new[] { new Card("X", 1), new Card("Y", 1), new Card("Z", 1)});

        Assert.Equal(3, deck.Size);

        var d1 = deck.Draw()!;
        deck.Discard(d1);
        Assert.Equal(3, deck.Size); // 2 draw + 1 discard
    }

    [Fact]
    public void AddCardAddsToBottom() {
        var a = new Card("A", 1);
        var b = new Card("B", 1);
        var deck = new Deck(new[] { a });

        deck.AddCard(b);
        var d1 = deck.Draw();
        var d2 = deck.Draw();

        Assert.Contains(d1, new[] { a, b });
        Assert.Contains(d2, new[] { a, b });
        Assert.NotEqual(d1, d2);
    }
}
