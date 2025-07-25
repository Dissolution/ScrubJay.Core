﻿namespace ScrubJay.Debugging;

[PublicAPI]
public sealed class LapTimer
{
    private readonly List<Lap> _laps = [];

    public string Name { get; init; }

    public LapTimer() : this(null) { }
    public LapTimer(string? name)
    {
        Name = name ?? nameof(LapTimer);
    }

    public Lap StartLap(string? name = null)
    {
        var lap = new Lap(name ?? $"Lap #{_laps.Count}");
        lap.Start();
        _laps.Add(lap);
        return lap;
    }

    public override string ToString()
    {
        if (_laps.Count == 0)
            return "LapTimer: Empty";

        return TextBuilder.New
            .EnumerateFormatAndDelimitLines(_laps)
            .ToStringAndDispose();
    }
}

[MustDisposeResource(false)]
public sealed class Lap : IDisposable
{
    private readonly Stopwatch _stopwatch = new();

    public string Name { get; }

    public bool IsRunning => _stopwatch.IsRunning;

    public TimeSpan Elapsed => _stopwatch.Elapsed;

    public Lap() : this(null) { }

    public Lap(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            Name = $"Lap {Guid.NewGuid():D}";
        }
        else
        {

            Name = name!;
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
