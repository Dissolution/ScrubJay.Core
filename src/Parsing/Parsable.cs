namespace ScrubJay.Parsing;

[PublicAPI]
public static class Parsable
{
#if NET7_0_OR_GREATER
    public static P Parse<P>(string str, IFormatProvider? provider = null)
        where P : IParsable<P> => P.Parse(str, provider);

    public static P Parse<P>(text text, IFormatProvider? provider = null)
        where P : ISpanParsable<P> => P.Parse(text, provider);

    public static bool TryParse<P>(
        [NotNullWhen(true)] string? str,
        [MaybeNullWhen(false)] out P result)
        where P : IParsable<P> => P.TryParse(str, null, out result);

    public static bool TryParse<P>(
        text text,
        [MaybeNullWhen(false)] out P result)
        where P : ISpanParsable<P> => P.TryParse(text, null, out result);
#endif
}