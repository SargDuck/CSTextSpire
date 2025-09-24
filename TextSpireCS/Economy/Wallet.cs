using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSpireCS.Economy;

public sealed class Wallet {
    public static Wallet? Current { get; private set; }
    public static void Attach(Wallet w) => Current = w;

    public int Gold { get; private set; }
    public Wallet(int startingGold = 0) { Gold = startingGold; }

    public void Add(int amount) { if (amount > 0) Gold += amount; }
    public bool Spend(int amount) {
        if (amount <= 0) return true;
        if (Gold < amount) return false;
        Gold -= amount; return true;
    }
}