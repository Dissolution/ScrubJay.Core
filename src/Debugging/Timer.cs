namespace ScrubJay.Debugging;

public static class Timer
{
    public static TimeSpan Time(Action action)
    {
        var timer = Stopwatch.StartNew();
        action();
        return timer.Elapsed;
    }

    public static (TResult Result, TimeSpan Elapsed) Time<TResult>(Func<TResult> func)
    {
        var timer = Stopwatch.StartNew();
        var result = func();
        return (result, timer.Elapsed);
    }

    public static async Task<TimeSpan> Time(Func<Task> asyncAction)
    {
        var timer = Stopwatch.StartNew();
        var task = asyncAction();
        await task;
        return timer.Elapsed;
    }

    public static async Task<(TResult Result, TimeSpan Elapsed)> Time<TResult>(Func<Task<TResult>> asyncFunc)
    {
        var timer = Stopwatch.StartNew();
        var task = asyncFunc();
        var result = await task;
        return (result, timer.Elapsed);
    }

#if !NET481 && !NETSTANDARD2_0
    public static async ValueTask<TimeSpan> Time(Func<ValueTask> asyncAction)
    {
        var timer = Stopwatch.StartNew();
        var task = asyncAction();
        await task;
        return timer.Elapsed;
    }
    
    public static async ValueTask<(TResult Result, TimeSpan Elapsed)> Time<TResult>(Func<ValueTask<TResult>> asyncFunc)
    {
        var timer = Stopwatch.StartNew();
        var task = asyncFunc();
        var result = await task;
        return (result, timer.Elapsed);
    }
#endif
}