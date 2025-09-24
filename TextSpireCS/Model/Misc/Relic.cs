using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSpireCS.Model.Misc;

public enum RelicKind { StartArmor, GoldBoost, ExtraHp }

public sealed class Relic {
    public RelicKind Kind { get; }
    public string Name { get; }
    public Relic(RelicKind kind, string name) {
        Kind = kind; Name = name;
    }
}