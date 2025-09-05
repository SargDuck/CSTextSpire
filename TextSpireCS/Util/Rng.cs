using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TextSpireCS.Util;

// Provides a shared Random generator for the whole game.
// Wraps Random.Next(min, max) with a lock to ensure thread safety.
// Ensures that concurrent access from multiple threads don't break randomness.
public static class Rng
{
    private static readonly object _gate = new();
    private static readonly Random _rand = new();

    public static Random Shared => _rand;

    public static int Next(int min, int max)
    {
        lock (_gate) return _rand.Next(min, max);
    }

}
