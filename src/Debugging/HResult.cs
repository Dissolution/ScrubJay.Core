namespace ScrubJay.Debugging;

[PublicAPI]
[StructLayout(LayoutKind.Explicit, Size = 4)]
public readonly struct HResult :
#if NET7_0_OR_GREATER
    IEqualityOperators<HResult, HResult, bool>,
#endif
    IEquatable<HResult>,
    IFormattable
{
    private const uint FAIL_MASK = 0b_10000000_00000000_00000000_00000000;
    private const uint FACI_MASK = 0b_01111111_11111111_00000000_00000000;
    private const uint CODE_MASK = 0b_00000000_00000000_11111111_11111111;

    public static implicit operator HResult(int hresult) => new(hresult);
    public static implicit operator HResult(uint hresult) => new(hresult);

    public static bool operator ==(HResult left, HResult right) => left.Equals(right);
    public static bool operator !=(HResult left, HResult right) => !left.Equals(right);


    [FieldOffset(0)]
    private readonly uint _hresult;

    public bool IsFailure
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (_hresult & FAIL_MASK) > 0U;
    }

    public ushort Facility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (ushort)((_hresult & FACI_MASK) >> 16);
    }

    public ushort Code
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (ushort)(_hresult & CODE_MASK);
    }

    public Uri HelpLink => new($"https://www.hresult.info/Search?q=0x{_hresult:X8}");

    public HResult(int hresult)
    {
        _hresult = (uint)hresult;
    }

    public HResult(uint hresult)
    {
        _hresult = hresult;
    }

    public bool Equals(HResult hresult) => _hresult == hresult._hresult;

    public bool Equals(uint hresult) => _hresult == hresult;

    public bool Equals(int hresult) => _hresult == (uint)hresult;

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj switch
        {
            HResult hresult => Equals(hresult),
            uint uhr => Equals(uhr),
            int hr => Equals(hr),
            _ => false,
        };
    }

    public override int GetHashCode() => (int)_hresult;

    public string ToString(string? format, IFormatProvider? provider = null)
    {
        return format switch
        {
            "X" => $"0x{_hresult:X8}",
            "I" => ((int)_hresult).ToString(format, provider),
            "U" => _hresult.ToString(format, provider),
            _ => $"""
                0x{_hresult:X8} {(IsFailure ? "FAIL" : "")}
                Facility: {Facility}  Code: {Code}
                """,
        };
    }

    public override string ToString() => ToString(null, null);
}
