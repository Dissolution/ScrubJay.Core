#pragma warning disable CA1008, CA2217, CA1034, CA2225

namespace ScrubJay.Text.Comparison;

public abstract record class StringMatch
{
    public static implicit operator StringMatch(StringComparison stringComparison) => stringComparison switch
    {
        StringComparison.CurrentCulture => new Current(),
        StringComparison.CurrentCultureIgnoreCase => new Current()
        {
            IgnoreCase = true,
        },
        StringComparison.InvariantCulture => new Invariant(),
        StringComparison.InvariantCultureIgnoreCase => new Invariant()
        {
            IgnoreCase = true,
        },
        StringComparison.Ordinal => new Ordinal(),
        StringComparison.OrdinalIgnoreCase => new Ordinal()
        {
            IgnoreCase = true,
        },
        _ => throw new UnreachableException(),
    };

    public bool IgnoreCase { get; init; }
    public bool StartsWith { get; init; }
    public bool EndsWith { get; init; }
    public bool Contains { get; init; }

    public bool IsExact => !IgnoreCase && !StartsWith && !EndsWith && !Contains;
    public abstract StringComparison StringComparison { get; }

    public sealed record class Ordinal : StringMatch
    {
        public override StringComparison StringComparison
            => IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
    }

    public sealed record class Invariant : StringMatch
    {
        public override StringComparison StringComparison
            => IgnoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;
    }

    public sealed record class Current : StringMatch
    {
        public override StringComparison StringComparison
            => IgnoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture;
    }
}


