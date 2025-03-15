// ReSharper disable InvokeAsExtensionMethod

namespace ScrubJay.Text.Comparison;

public sealed class OrdinalIgnoreCaseTextComparer : StringComparisonTextComparer
{
    public OrdinalIgnoreCaseTextComparer() : base(StringComparison.OrdinalIgnoreCase)
    {
    }

    public override int Compare(string? x, string? y)
        => string.Compare(x, y, StringComparison.OrdinalIgnoreCase);

    public override int Compare(text x, text y)
        => MemoryExtensions.CompareTo(x, y, StringComparison.OrdinalIgnoreCase);

    public override bool Equals(string? x, string? y)
        => string.Equals(x, y, StringComparison.OrdinalIgnoreCase);

    public override bool Equals(text x, text y)
        => MemoryExtensions.Equals(x, y, StringComparison.OrdinalIgnoreCase);


    public override int GetHashCode(char ch)
    {
#if NETFRAMEWORK || NETSTANDARD || NETCOREAPP3_1
        return ch.AsString().GetHashCode(StringComparison.OrdinalIgnoreCase);
#else
        return string.GetHashCode(ch.AsSpan(), StringComparison.OrdinalIgnoreCase);
#endif
    }

    public override int GetHashCode(string? str)
    {
        if (str is null)
            return Hasher.NullHash;
#if NETFRAMEWORK || NETSTANDARD || NETCOREAPP3_1
        return str.GetHashCode(StringComparison.OrdinalIgnoreCase);
#else
        return string.GetHashCode(str, StringComparison.OrdinalIgnoreCase);
#endif
    }

    public override int GetHashCode(char[]? chars)
    {
        if (chars is null)
            return Hasher.NullHash;
#if NETFRAMEWORK || NETSTANDARD || NETCOREAPP3_1
        return chars.AsString().GetHashCode(StringComparison.OrdinalIgnoreCase);
#else
        return string.GetHashCode(chars.AsSpan(), StringComparison.OrdinalIgnoreCase);
#endif
    }

    public override int GetHashCode(scoped text text)
    {
#if NETFRAMEWORK || NETSTANDARD || NETCOREAPP3_1
        return text.AsString().GetHashCode(StringComparison.OrdinalIgnoreCase);
#else
        return string.GetHashCode(text, StringComparison.OrdinalIgnoreCase);
#endif
    }
}
