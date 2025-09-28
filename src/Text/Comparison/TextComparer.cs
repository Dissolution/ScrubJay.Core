namespace ScrubJay.Text.Comparison;

[PublicAPI]
public abstract class TextComparer :
    ITextEqualityComparer,
    IEqualityComparer,
    ITextComparer,
    IComparer
{
    public static OrdinalTextComparer Ordinal { get; } = new OrdinalTextComparer();

    public static OrdinalIgnoreCaseTextComparer OrdinalIgnoreCase { get; } = new OrdinalIgnoreCaseTextComparer();

    public static CulturedTextComparer Invariant { get; } = new CulturedTextComparer(StringComparison.InvariantCulture);

    public static CulturedTextComparer InvariantIgnoreCase { get; } = new CulturedTextComparer(StringComparison.InvariantCultureIgnoreCase);

    public static CulturedTextComparer Current { get; } = new CulturedTextComparer(StringComparison.CurrentCulture);

    public static CulturedTextComparer CurrentIgnoreCase { get; } = new CulturedTextComparer(StringComparison.CurrentCultureIgnoreCase);

    public static StringComparisonTextComparer FromComparison(StringComparison comparison) => comparison switch
    {
        StringComparison.CurrentCulture => Current,
        StringComparison.CurrentCultureIgnoreCase => CurrentIgnoreCase,
        StringComparison.InvariantCulture => Invariant,
        StringComparison.InvariantCultureIgnoreCase => InvariantIgnoreCase,
        StringComparison.Ordinal => Ordinal,
        StringComparison.OrdinalIgnoreCase => OrdinalIgnoreCase,
        _ => throw new ArgumentOutOfRangeException(nameof(comparison), comparison, null),
    };


    int IComparer.Compare(object? x, object? y)
    {
        if (x is null)
            return y is null ? 0 : -1;
        if (y is null)
            return 1;

        if (TextHelper.TryUnboxText(x, out text xText))
        {
            if (TextHelper.TryUnboxText(y, out text yText))
            {
                return Compare(xText, yText);
            }
            else
            {
                throw Ex.Arg(y, $"Cannot compare non-textual object containing `{x:@T}`");
            }
        }
        else
        {
            throw Ex.Arg(y, $"Cannot compare non-textual object containing `{y:@T}`");
        }
    }

    bool IEqualityComparer.Equals(object? x, object? y)
    {
        if (x is null)
            return y is null;
        if (y is null)
            return false;

        if (TextHelper.TryUnboxText(x, out text xText))
        {
            if (TextHelper.TryUnboxText(y, out text yText))
            {
                return Equals(xText, yText);
            }
            else
            {
                throw Ex.Arg(y, $"Cannot equate non-textual object containing `{y:@T}`");
            }
        }
        else
        {
            throw Ex.Arg(x, $"Cannot equate non-textual object containing `{x:@T}`");
        }
    }

    int IEqualityComparer.GetHashCode(object? obj)
    {
        if (obj is null)
            return Hasher.NullHash;
        if (TextHelper.TryUnboxText(obj, out text text))
            return Hasher.HashMany<char>(text);
        throw Ex.Arg(obj, $"Cannot get a hashcode for non-textual object containing `{obj:@T}`");
    }


    public virtual int Compare(char x, char y)
        => Compare(x.AsSpan(), y.AsSpan());

    public virtual int Compare(string? x, string? y)
    {
        if (x is null)
            return y is null ? 0 : -1;
        if (y is null)
            return 1;
        return Compare(x.AsSpan(), y.AsSpan());
    }

    public virtual int Compare(char[]? x, char[]? y)
    {
        if (x is null)
            return y is null ? 0 : -1;
        if (y is null)
            return 1;
        return Compare(x.AsSpan(), y.AsSpan());
    }

    public abstract int Compare(text x, text y);


    public virtual bool Equals(char x, char y)
        => Equals(x.AsSpan(), y.AsSpan());

    public virtual bool Equals(string? x, string? y)
    {
        if (x is null)
            return y is null;
        if (y is null)
            return false;
        return Equals(x.AsSpan(), y.AsSpan());
    }

    public virtual bool Equals(char[]? x, char[]? y)
    {
        if (x is null)
            return y is null;
        if (y is null)
            return false;
        return Equals(x.AsSpan(), y.AsSpan());
    }

    public abstract bool Equals(text x, text y);


    public virtual int GetHashCode(char ch)
        => GetHashCode(ch.AsSpan());

    public virtual int GetHashCode(string? str)
    {
        if (str is null)
            return Hasher.NullHash;
        return GetHashCode(str.AsSpan());
    }

    public virtual int GetHashCode(char[]? chars)
    {
        if (chars is null)
            return Hasher.NullHash;
        return GetHashCode(chars.AsSpan());
    }

    public abstract int GetHashCode(text text);
}
