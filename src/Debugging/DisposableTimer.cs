namespace ScrubJay.Debugging;

public class DisposableTimer : IDisposable
{
    private readonly Stopwatch _stopwatch;

    public TimeSpan Elapsed => _stopwatch.Elapsed;
    
    public DisposableTimer()
    {
        _stopwatch = Stopwatch.StartNew();
    }

    public void Dispose()
    {
        _stopwatch.Stop();
    }
}