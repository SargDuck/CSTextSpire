using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace TextSpireCS.UI;

// Prints an ASCII banner (banner.txt).
public static class BannerPrinter {
    public static void Print() {
        try {

            // Looks for an embedded resource called TextSpireCS.UI.banner.txt inside the assembly.
            var asm = Assembly.GetExecutingAssembly();
            using Stream? stream = asm.GetManifestResourceStream("TextSpireCS.UI.banner.txt");
            if (stream == null) return;

            using var reader = new StreamReader(stream, Encoding.UTF8);
            string? line;
            while ((line = reader.ReadLine()) != null) {
                Console.WriteLine(line);
            }
        }
        catch {
            // Ignored since banner is optional.
        }
    }
}
