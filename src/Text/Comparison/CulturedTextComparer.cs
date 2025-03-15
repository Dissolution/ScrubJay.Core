using System.Globalization;

namespace ScrubJay.Text.Comparison;

public sealed class CulturedTextComparer : StringComparisonTextComparer
{
    public CultureInfo Culture { get; }

    public CompareInfo CompareInfo => Culture.CompareInfo;

    public TextInfo TextInfo => Culture.TextInfo;

    public CompareOptions CompareOptions { get; }

    public CulturedTextComparer(StringComparison stringComparison) : base(stringComparison)
    {
        switch (stringComparison)
        {
            case StringComparison.CurrentCulture:
            {
                Culture = CultureInfo.CurrentCulture;
                CompareOptions = CompareOptions.None;
                break;
            }
            case StringComparison.CurrentCultureIgnoreCase:
            {
                Culture = CultureInfo.CurrentCulture;
                CompareOptions = CompareOptions.IgnoreCase;
                break;
            }
            case StringComparison.InvariantCulture:
            {
                Culture = CultureInfo.InvariantCulture;
                CompareOptions = CompareOptions.None;
                break;
            }
            case StringComparison.InvariantCultureIgnoreCase:
            {
                Culture = CultureInfo.InvariantCulture;
                CompareOptions = CompareOptions.IgnoreCase;
                break;
            }
            case StringComparison.Ordinal:
            case StringComparison.OrdinalIgnoreCase:
            default:
                throw new ArgumentOutOfRangeException(nameof(stringComparison), stringComparison, null);
        }
    }

    public override int Compare(char x, char y)
    {
#if NETFRAMEWORK || NETSTANDARD || NETCOREAPP3_1
        return CompareInfo.Compare(x.AsString(), y.AsString(), CompareOptions);
#else
        return CompareInfo.Compare(x.AsSpan(), y.AsSpan(), CompareOptions);
#endif
    }

    public override int Compare(string? x, string? y)
    {
        return CompareInfo.Compare(x, y, CompareOptions);
    }

    public override int Compare(char[]? x, char[]? y)
    {
#if NETFRAMEWORK || NETSTANDARD || NETCOREAPP3_1
        return CompareInfo.Compare(x.AsString(), y.AsString(), CompareOptions);
#else
        return CompareInfo.Compare(x.AsSpan(), y.AsSpan(), CompareOptions);
#endif
    }

    public override int Compare(text x, text y)
    {
#if NETFRAMEWORK || NETSTANDARD || NETCOREAPP3_1
        return CompareInfo.Compare(x.AsString(), y.AsString(), CompareOptions);
#else
        return CompareInfo.Compare(x, y, CompareOptions);
#endif
    }


    public override bool Equals(char x, char y)
    {
#if NETFRAMEWORK || NETSTANDARD || NETCOREAPP3_1
        return CompareInfo.Compare(x.AsString(), y.AsString(), CompareOptions) == 0;
#else
        return CompareInfo.Compare(x.AsSpan(), y.AsSpan(), CompareOptions) == 0;
#endif
    }

    public override bool Equals(string? x, string? y)
    {
        return CompareInfo.Compare(x, y, CompareOptions) == 0;
    }

    public override bool Equals(char[]? x, char[]? y)
    {
#if NETFRAMEWORK || NETSTANDARD || NETCOREAPP3_1
        return CompareInfo.Compare(x.AsString(), y.AsString(), CompareOptions) == 0;
#else
        return CompareInfo.Compare(x.AsSpan(), y.AsSpan(), CompareOptions) == 0;
#endif
    }

    public override bool Equals(text x, text y)
    {
#if NETFRAMEWORK || NETSTANDARD || NETCOREAPP3_1
        return CompareInfo.Compare(x.AsString(), y.AsString(), CompareOptions) == 0;
#else
        return CompareInfo.Compare(x, y, CompareOptions) == 0;
#endif
    }


    public override int GetHashCode(char ch)
    {
#if NETFRAMEWORK || NETSTANDARD || NETCOREAPP3_1
        return CompareInfo.GetHashCode(ch.AsString(), CompareOptions);
#else
        return CompareInfo.GetHashCode(ch.AsSpan(), CompareOptions);
#endif
    }

    public override int GetHashCode(string? str)
    {
        if (str is null)
            return Hasher.NullHash;
        return CompareInfo.GetHashCode(str, CompareOptions);
    }

    public override int GetHashCode(char[]? chars)
    {
        if (chars is null)
            return Hasher.NullHash;
#if NETFRAMEWORK || NETSTANDARD || NETCOREAPP3_1
        return CompareInfo.GetHashCode(chars.AsString(), CompareOptions);
#else
        return CompareInfo.GetHashCode(chars.AsSpan(), CompareOptions);
#endif
    }

    public override int GetHashCode(text text)
    {
#if NETFRAMEWORK || NETSTANDARD || NETCOREAPP3_1
        return CompareInfo.GetHashCode(text.AsString(), CompareOptions);
#else
        return CompareInfo.GetHashCode(text, CompareOptions);
#endif
    }
}
