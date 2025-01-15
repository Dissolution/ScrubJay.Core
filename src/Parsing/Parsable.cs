namespace ScrubJay.Text.Parsing;

[PublicAPI]
public static class Parsable
{
#if NET7_0_OR_GREATER
    public static TParsable Parse<TParsable>(string str, IFormatProvider? provider = default)
        where TParsable : IParsable<TParsable>
    {
        return TParsable.Parse(str, provider);
    }

    public static TSpanParsable Parse<TSpanParsable>(text text, IFormatProvider? provider = default)
        where TSpanParsable : ISpanParsable<TSpanParsable>
    {
        return TSpanParsable.Parse(text, provider);
    }

    public static bool TryParse<TParsable>(
        [NotNullWhen(true)] string? str,
        [MaybeNullWhen(false)] out TParsable result)
        where TParsable : IParsable<TParsable>
    {
        return TParsable.TryParse(str, default, out result);
    }

    public static bool TryParse<TSpanParsable>(
        text text,
        [MaybeNullWhen(false)] out TSpanParsable result)
        where TSpanParsable : ISpanParsable<TSpanParsable>
    {
        return TSpanParsable.TryParse(text, default, out result);
    }
#endif
}