namespace ScrubJay.Debugging;

[PublicAPI]
[MustDisposeResource(false)]
public sealed class Profiler : IDisposable
{
    private readonly Stopwatch _masterWatch = Stopwatch.StartNew();
    private readonly List<ProfileSection> _sections = [];

    internal long CurrentTicks => _masterWatch.ElapsedTicks;

    public string? Name { get; }

    public TimeSpan TotalElapsed => _masterWatch.Elapsed;
    public IReadOnlyList<ProfileSection> Sections => _sections;

    public Profiler(string? name = null)
    {
        this.Name = name;
    }


    public ProfileSection Profile(string? name)
    {
        var section = new ProfileSection(this, name);
        _sections.Add(section);
        return section;
    }

    public void Stop()
    {
        _masterWatch.Stop();
        foreach (var section in _sections)
        {
            section.Stop();
        }
    }

    public void Dispose() => Stop();

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