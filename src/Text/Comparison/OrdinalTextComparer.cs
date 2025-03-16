// ReSharper disable InvokeAsExtensionMethod


// ReSharper disable ArrangeMethodOrOperatorBody

namespace ScrubJay.Text.Comparison;

public sealed class OrdinalTextComparer : StringComparisonTextComparer
{
    public OrdinalTextComparer() : base(StringComparison.Ordinal)
    {
    }

    public override int Compare(char x, char y)
        => x.CompareTo(y);

    public override int Compare(string? x, string? y)
        => string.CompareOrdinal(x, y);

    public override int Compare(text x, text y)
        => MemoryExtensions.SequenceCompareTo<char>(x, y);

    public override bool Equals(char x, char y)
        => x.Equals(y);

    public override bool Equals(string? x, string? y)
        => string.Equals(x, y);

    public override bool Equals(text x, text y)
        => MemoryExtensions.SequenceEqual<char>(x, y);


    public override int GetHashCode(char ch)
        => Hasher.Hash(ch);

    public override int GetHashCode(string? str)
        => str is null ? Hasher.NullHash : Hasher.HashMany<char>(str.AsSpan());

    public override int GetHashCode(char[]? chars)
        => Hasher.HashMany<char>(chars);

    public override int GetHashCode(scoped text text)
        => Hasher.HashMany<char>(text);
}
