using System.Threading;

namespace TextSpireCS.Util;

public static class ThreadUtils
{
    public static void SleepQuietly(int ms)
    {
        try {
            if (ms <= 0) return;
            Thread.Sleep(ms);
        }
        catch {
            // ignored
        }
    }
}
