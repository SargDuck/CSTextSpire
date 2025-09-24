using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextSpireCS.UI;

public static class ManualScreen {
    private static readonly string Basics = """
        === Basics ===
        - Real-time combat: enemies attack on intervals.
        - Commands: play N, use, status, help, quit, manual.
        - Rooms: Rest, Offense, Defense, Monster, Shop, Event.
        """;

    private static readonly string Commands = """
        === Commands ===
        play N   - use drawn card on enemy N
        use      - drink first potion
        status   - show HP/deck/inventory
        help     - quick command list
        manual   - this manual
        quit     - end combat
        """;

    private static readonly string Rooms = """
        === Rooms ===
        Rest: heal + potion
        Offense: add Strike to deck
        Defense: add Defend to deck
        Monster: combat
        Shop: buy items with gold
        Event: random choice with outcomes
        """;

    public static void Show() {
        Console.WriteLine(Basics);
        Console.WriteLine(Commands);
        Console.WriteLine(Rooms);
    }
}
