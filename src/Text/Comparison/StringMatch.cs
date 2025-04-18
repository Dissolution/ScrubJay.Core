﻿#pragma warning disable CA1008, CA2217, CA1034, CA2225

namespace ScrubJay.Text.Comparison;

[PublicAPI]
public sealed record class StringMatch
{
    public StringComparison StringComparison { get; init; } = StringComparison.Ordinal;

    public bool StartsWith { get; init; } = false;

    public bool EndsWith { get; init; } = false;

    public bool Contains { get; init; } = false;


    public bool IgnoresCase => (((int)StringComparison % 2) != 0);

    public bool IsExact => (StringComparison == StringComparison.Ordinal) &&
        !StartsWith && !EndsWith && !Contains;


    public StringMatch() { }

    public StringMatch(StringComparison comparison)
    {
        this.StringComparison = comparison;
    }
}


