using System;

namespace TextSpireCS.IO;

// Parses a line of console input into a command tuple (string cmd, int arg, bool ok).
// play N => "play", "arg = N", and "ok = true" if N parses an int.
// use, quit, status, help => returns the command with arg = 0" and "ok = true".
// Anything else => returns an empty command with "arg = 0" and "ok = false".
public static class CommandParser {

    // Unused Enum
    // public enum CommandKind { None, Play, Use, Quit, Status, Help }

    public static (string cmd, int arg, bool ok) Parse(string line) {

        // null safe parse that removes leading/trailing spaces/newlines.
        var parts = (line ?? "").Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // Empty input => not ok.
        if (parts.Length == 0) return ("", 0, false);

        // Case-insensitive commands: PLAY, play, plAY all work.
        var cmd = parts[0].ToLowerInvariant();

        // For play, parses the second token as an int.
        // If missing, it flags ok : false.
        // If present but not an int or under 1, it sets "args = 0" and "ok = false".
        if (cmd == "play") {
            if (parts.Length < 2 || !int.TryParse(parts[1], out var n) || n <= 0)
                return (cmd, 0, false);
            return (cmd, n, true);
        }
        // Commands with no args are always ok.
        if (cmd is "use" or "quit" or "status" or "help")
            return (cmd, 0, true);

        // Anything else is invalid
        return ("", 0, false);
    }
}
