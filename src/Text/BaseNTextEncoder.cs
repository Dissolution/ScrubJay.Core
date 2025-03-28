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
            throw new ArgumentException(default, nameof(symbols));
        _radix = symbols.Length;
        _symbols = symbols.ToCharArray();
    }

    public BaseNTextEncoder(params char[] symbols)
    {
        if (symbols.Distinct().Count() != symbols.Length)
            throw new ArgumentException(default, nameof(symbols));
        _radix = symbols.Length;
        _symbols = symbols;
    }

    public Result<string> TryEncode(BigInteger iBig)
    {
#if NET5_0_OR_GREATER
        long bitLength = iBig.GetBitLength();
        if (bitLength > int.MaxValue)
            return new ArgumentException($"BigInt would require {bitLength} characters, which is larger than int.MaxValue", nameof(iBig));
#else
        long bitLength = 1024L;
#endif

        using TextBuffer buffer = stackalloc char[(int)bitLength];
        text symbols = _symbols;
        BigInteger len = symbols.Length;
        do
        {
            var charsIndex = iBig % len;
            Debug.Assert((charsIndex >= BigInteger.Zero) && (charsIndex < len));
            buffer.Add(symbols[(int)charsIndex]);
            iBig /= len;
        } while (iBig > BigInteger.Zero);
        return Ok(buffer.ToString());
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
