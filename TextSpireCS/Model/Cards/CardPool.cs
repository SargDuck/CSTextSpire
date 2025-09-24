using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSpireCS.Model.Cards;

public static class CardPool {
    // Basic pool for rewards
    public static readonly List<Card> BasicPool = new() {
        new("Strike", 6),
        new("Defend", 0),

        new("Bash", 8),         // stronger strike. Applies Vulnerable +1 to target
        new("Cleave", 4),       // hit all enemies for 4
        new("Pommel", 7),       // stronger strike
        new("Slice", 5),        // weaker strike
        new("Smash", 10),       // very strong strike
        new("Reinforce", 0),    // stronger defend
        new("Shock", 3),        // weaker strike. Applies Weak +1 to target
    };
}