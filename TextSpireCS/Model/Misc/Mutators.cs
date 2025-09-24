using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSpireCS.Model.Misc;

public enum Mutator { None, Rich }

public static class MutatorRules {
    public static Mutator Current { get; private set; } = Mutator.None;
    public static void Set(Mutator m) => Current = m;
}