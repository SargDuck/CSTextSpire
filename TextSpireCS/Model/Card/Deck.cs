using System;
using System.Collections.Generic;
using System.Linq;

namespace TextSpireCS.Model.Card;

// Models a Slay the Spire-style deck.
// Draw pile and discard pile.
// Auto refills and draws when empty.
// Shuffles like a physical card deck.
// Supports adding new cards (from rooms) and discarding played ones.
// Exposes deck size and snapshot of contents
public sealed class Deck {
    
    // Two seperate piles with O(1) indexing and shuffling.
    private readonly List<Card> _drawPile;
    private readonly List<Card> _discard = new();

    // Uses the gloval RNG for consistent randomness across the game.
    private readonly Random _rng = Util.Rng.Shared;
    
    // Initializes with a starter deck and shuffles on creation.
    public Deck(IEnumerable<Card> startingCards) {
        _drawPile = startingCards.ToList();
        ShuffleDrawPile();
    }

    // If draw pile is empty, auto refill from discard pile.
    // Satefy guard for null edge case by returning null instead of crashing.
    public Card? Draw() {
        if (_drawPile.Count == 0)
            RefillAndShuffle();
        if (_drawPile.Count == 0)
            return null;

        // Draws from the top, index 0.
        // Removes from the pile, but DOES NOT discard. 
        // Caller calls discard,
        var c = _drawPile[0];
        _drawPile.RemoveAt(0);
        return c;
    }

    // Shuffle that brings the discard pile back if draw pile is empty
    public void Shuffle() {
        if (_drawPile.Count == 0)
            RefillAndShuffle();
        else
            ShuffleDrawPile();
    }

    // Adds to the bottom of the draw pile.
    public void AddCard(Card card) => _drawPile.Add(card);

    // Discard that is manually called by the caller (e.g. after card is played successfully)
    public void Discard(Card card) => _discard.Add(card);

    // Total deck size (draw + discard piles)
    public int Size => _drawPile.Count + _discard.Count;

    // Immutable snapshot of all cards
    public List<Card> GetAllCards()
        => _drawPile.Concat(_discard).ToList();

    // Fisher-Yates shuffle. 
    // For i from list size - 1 to 1, pick a random index j uniformly from 0 to i inclusive,
    // Swap list[i] and list[j]
    private void ShuffleDrawPile() {
        for (int i = _drawPile.Count - 1; i > 0; i--) {
            int j = _rng.Next(i + 1);
            (_drawPile[i], _drawPile[j]) = (_drawPile[j], _drawPile[i]);
        }
    }

    // Moves discard pile back into draw, then shuffles.
    private void RefillAndShuffle() {
        if (_discard.Count == 0) return;
        _drawPile.AddRange(_discard);
        _discard.Clear();
        ShuffleDrawPile();
    }
}
