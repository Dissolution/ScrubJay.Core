using System.Text;

namespace ScrubJay.Debugging;

[PublicAPI]
public sealed class Profiler : IDisposable
{
    private readonly Stopwatch _masterWatch = Stopwatch.StartNew();
    private readonly List<ProfileSection> _sections = [];

    public string? Name { get; }

    /// <summary>
    /// Gets the total elapsed time since the ProfileTimer was created.
    /// </summary>
    public TimeSpan TotalElapsed
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _masterWatch.Elapsed;
    }

    public long CurrentTicks
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _masterWatch.ElapsedTicks;
    }

    /// <summary>
    /// Gets read-only access to all profiled sections.
    /// </summary>
    public IReadOnlyList<ProfileSection> Sections => _sections;

    public Profiler(string? name = null)
    {
        this.Name = name;
    }

    public ProfileSection Profile(string? name)
    {
        var ticks = _masterWatch.ElapsedTicks;
        var section = new ProfileSection(this, name, ticks);
        _sections.Add(section);
        return section;
    }

    /// <summary>
    /// Stop all running Sections
    /// </summary>
    public void Dispose()
    {
        var ticks = _masterWatch.ElapsedTicks;
        _masterWatch.Stop();

        foreach (var section in _sections)
        {
            section.Dispose(ticks);
        }
    }

    public override string ToString()
    {
        using var builder = new TextBuilder();

        builder.IfNotEmpty(Name, static (tb, name) => tb.Append(name).Write(' '))
            .AppendLine("Profiler")
            .Repeat(13, '-').NewLine();

        if (_sections.Count == 0)
        {
            builder.Write("No Sections profiled");
            return builder.ToString();
        }

        builder.AppendLine($"Total: {TotalElapsed:g}");

        // ms checkpoint for determining nesting
        var checkpoint = TotalElapsed.TotalMilliseconds;

        for (int i = 0; i < _sections.Count; i++)
        {
            var section = _sections[i];
            var elapsed = section.Elapsed;
            var percentage = checkpoint > 0d ? (elapsed.TotalMilliseconds / checkpoint) * 100d : 0d;

            // Calculate depth: if this section starts before the previous one ends, it's nested
            int depth = 0;
            for (int j = i - 1; j >= 0; j--)
            {
                var prev = _sections[j];
                if (section.StartTicks < prev.EndTicks || prev.IsRunning)
                {
                    depth++;
                }
                else
                {
                    break; // Found a section that ended before this one started
                }
            }

            builder.Repeat(depth * 2, ' ')
                .IfNotEmpty(section.Name, static (tb, name) => tb.Append('-').Append(name).Write(": "),
                    static tb => tb.Write("-- "))
                .Append($"{elapsed:g} ({percentage:F2}%)")
                .If(section.IsRunning, " [Running]")
                .NewLine();
        }

        return builder.ToString();
    }
}

/// <summary>
/// Represents a timed section that can be queried for elapsed time and status.
/// Dispose to stop timing.
/// </summary>
[PublicAPI]
[MustDisposeResource(false)]
public sealed class ProfileSection : IDisposable
{
    private readonly Profiler _timer;
    private readonly long _startTicks;
    private long _endTicks;

    public string Name { get; }

    /// <summary>
    /// Gets whether this section is currently running.
    /// </summary>
    public bool IsRunning => _endTicks < 0;

    /// <summary>
    /// Gets whether this section has been stopped.
    /// </summary>
    public bool IsStopped => _endTicks >= 0;

    /// <summary>
    /// Gets the start time in ticks (for calculating nesting depth).
    /// </summary>
    internal long StartTicks => _startTicks;

    /// <summary>
    /// Gets the end time in ticks (for calculating nesting depth).
    /// Returns -1 if still running.
    /// </summary>
    internal long EndTicks => _endTicks;

    /// <summary>
    /// Gets the elapsed time for this section.
    /// If still running, returns the time elapsed so far.
    /// </summary>
    public TimeSpan Elapsed
    {
        get
        {
            var endTicks = _endTicks < 0 ? _timer.GetCurrentTicks() : _endTicks;
            var elapsedTicks = endTicks - _startTicks;
            return TimeSpan.FromTicks((long)(elapsedTicks / (double)_timer.GetTickFrequency() * TimeSpan.TicksPerSecond));
        }
    }

    /// <summary>
    /// Gets the elapsed time in milliseconds.
    /// </summary>
    public double ElapsedMilliseconds => Elapsed.TotalMilliseconds;

    internal ProfileSection(Profiler timer, string name, long startTicks)
    {
        _timer = timer;
        Name = name;
        _startTicks = startTicks;
        _endTicks = -1; // Negative indicates running
    }

    /// <summary>
    /// Manually stops this section.
    /// </summary>
    public void Stop()
    {
        Dispose(_timer.CurrentTicks);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Dispose(long ticks)
    {
        if (_endTicks < 0) // Only stop if still running
        {
            _endTicks = ticks;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() => Stop();

    public override string ToString() =>
        $"{Name}: {Elapsed.TotalMilliseconds:F2}ms {(IsRunning ? "[RUNNING]" : "[STOPPED]")}";
}