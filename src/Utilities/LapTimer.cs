using ScrubJay.Text;

namespace ScrubJay.Utilities;

[PublicAPI]
[MustDisposeResource(false)]
public sealed class LapTimer : IDisposable
{
    private readonly List<Lap> _laps = [];

    public string Name { get; init; }

    public LapTimer() : this(null) { }
    public LapTimer(string? name)
    {
        this.Name = name ?? nameof(LapTimer);
    }

    public Lap StartLap(string? name = null)
    {
        var lap = new Lap(name ?? $"Lap #{_laps.Count}");
        lap.Start();
        _laps.Add(lap);
        return lap;
    }

    public void Dispose()
    {
        _laps.ForEach(lap => lap.Dispose());
        _laps.Clear();
    }

    public override string ToString()
    {
        if (_laps.Count == 0)
            return "LapTimer: Empty";

        var buffer = new Buffer<char>();
        foreach (var lap in _laps)
        {
            buffer.Write(lap);
            buffer.Write(Environment.NewLine);
        }
        return buffer.ToStringAndDispose();
    }
}

[MustDisposeResource(false)]
public sealed class Lap : IDisposable
{
    private readonly Stopwatch _stopwatch = new();

    public string Name { get; init; }

    public bool IsRunning => _stopwatch.IsRunning;

    public TimeSpan Elapsed => _stopwatch.Elapsed;

    public Lap() : this(null) { }

    public Lap(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            this.Name = $"Lap {Guid.NewGuid():D}";
        }
        else
        {

            this.Name = name!;
        }
    }

    public void Start() => _stopwatch.Start();

    public TimeSpan Restart()
    {
        _stopwatch.Stop();
        var elapsed = _stopwatch.Elapsed;
        _stopwatch.Start();
        return elapsed;
    }

    public TimeSpan Stop()
    {
        _stopwatch.Stop();
        return _stopwatch.Elapsed;
    }

    public void Dispose() => _stopwatch.Stop();

    public override string ToString() => $"{Name}: {(IsRunning ? "Running" : "Stopped")} - {Elapsed}";
}
