
namespace ScrubJay.Debugging;

[PublicAPI]
[MustDisposeResource(false)]
public sealed class ProfileSection : IDisposable
{
    private readonly Profiler _profiler;
    private readonly long _startTicks;
    private long _endTicks;

    internal long StartTicks => _startTicks;
    internal long EndTicks => _endTicks;

    public string? Name { get; }

    public bool IsRunning => _endTicks < 0;
    public bool IsStopped => _endTicks >= 0;

    public TimeSpan Elapsed
    {
        get
        {
            var endTicks = _endTicks < 0 ? _profiler.CurrentTicks : _endTicks;
            var elapsedTicks = endTicks - _startTicks;
            return TimeSpan.FromTicks(elapsedTicks);
        }
    }

    internal ProfileSection(Profiler profiler, string? name)
    {
        Name = name;
        _profiler = profiler;
        _startTicks = _profiler.CurrentTicks;
        _endTicks = -1; // Negative indicates running
    }

    public void Stop()
    {
        // only stop if still running
        if (_endTicks < 0)
        {
            _endTicks = _profiler.CurrentTicks;
        }
    }

    public void Dispose() => Stop();

    public override string ToString()
    {
        return TextBuilder.New
            .Append($"{_profiler.Name} Profiler section '{Name}': ")
            .If(_endTicks < 0, "Running for ", "Ran for ")
            .Render(Elapsed)
            .ToStringAndDispose();
    }
}