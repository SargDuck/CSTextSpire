using System;
using System.Diagnostics;
using TextSpireCS.Util;
using Xunit;

namespace TextSpireCS.Tests;

public class ThreadUtilsTests {
    [Fact]
    public void SleepQuietlyWaitsAtLeastRequestedTime() {
        var sw = Stopwatch.StartNew();
        ThreadUtils.SleepQuietly(50); // 50 ms
        sw.Stop();
        Assert.True(sw.ElapsedMilliseconds >= 45,
            $"Expected >= 45ms, got {sw.ElapsedMilliseconds}");
    }

    [Fact]
    public void SleepQuietlyDoesNotThrowOnNegativeInput() {
        Exception? ex = Record.Exception(() => ThreadUtils.SleepQuietly(-5));
        Assert.Null(ex);
    }

    [Fact]
    public void SleepQuietlyDoesNotThrowOnZero() {
        Exception? ex = Record.Exception(() => ThreadUtils.SleepQuietly(0));
        Assert.Null(ex);
    }
}
