namespace ScrubJay.Text;

public class BaseNTextEncoder
{
    public static BaseNTextEncoder Base10 { get; } = new BaseNTextEncoder("0123456789");

    public static BaseNTextEncoder Base16 { get; } = new BaseNTextEncoder("0123456789ABCDEF");

    public static BaseNTextEncoder Base62 { get; } = new("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz");

    private readonly int _radix;

    private readonly char[] _symbols;

    public BaseNTextEncoder(string symbols)
    {
        if (symbols.Distinct().Count() != symbols.Length)
            throw new ArgumentException(null, nameof(symbols));
        _radix = symbols.Length;
        _symbols = symbols.ToCharArray();
    }

    public BaseNTextEncoder(params char[] symbols)
    {
        if (symbols.Distinct().Count() != symbols.Length)
            throw new ArgumentException(null, nameof(symbols));
        _radix = symbols.Length;
        _symbols = symbols;
    }

    public Result<string> TryEncode(BigInteger iBig)
    {
        using var builder = new TextBuilder();
        text symbols = _symbols;
        BigInteger len = symbols.Length;
        do
        {
            var charsIndex = iBig % len;
            Debug.Assert((charsIndex >= BigInteger.Zero) && (charsIndex < len));
            builder.Append(symbols[(int)charsIndex]);
            iBig /= len;
        } while (iBig > BigInteger.Zero);

        // have to reverse
        builder.Written.Reverse();
        return Ok(builder.ToString());
    }

    public Result<BigInteger> TryDecode(string str)
    {
        if (Validate.IsNotEmpty(str).IsError(out var ex))
            return ex;

        BigInteger iBig = BigInteger.Zero;
        BigInteger radix = _radix;
        text symbols = _symbols;

        int len = str.Length;
        for (int i = 0; i < len; i++)
        {
            int symbolIndex = symbols.IndexOf(str[i]);
            if (symbolIndex < 0)
                return new ArgumentException($"Invalid symbol '{str[i]}'", nameof(str));

            iBig += (symbolIndex * BigInteger.Pow(radix, len - (i + 1)));
        }

        return Ok(iBig);
    }
}
