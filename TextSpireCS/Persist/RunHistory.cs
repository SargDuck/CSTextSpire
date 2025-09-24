using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSpireCS.Persist;
public sealed class RunEntry {
    public DateTime when { get; set; }
    public int floor { get; set; }
    public int potionsUsed { get; set; }
    public int goldAtEnd { get; set; }
    public string heroClass { get; set; } = "";
    public string notes { get; set; } = "";
}