#pragma warning disable CA1707

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

    /// <summary>
    /// Operation successful
    /// </summary>
    public static readonly HResult S_OK = 0x00000000;

    /// <summary>
    /// Completed without error, but only partial results were obtained
    /// </summary>
    public static readonly HResult S_FALSE = 0x00000001;

    /// <summary>
    /// Operation aborted
    /// </summary>
    public static readonly HResult E_ABORT = 0x80004004;

    /// <summary>
    /// General access denied error
    /// </summary>
    public static readonly HResult E_ACCESSDENIED = 0x80070005;

    /// <summary>
    /// Unspecified failure
    /// </summary>
    public static readonly HResult E_FAIL = 0x80004005;

    /// <summary>
    /// Handle that is not valid
    /// </summary>
    public static readonly HResult E_HANDLE = 0x80070006;

    /// <summary>
    /// One or more arguments are not valid
    /// </summary>
    public static readonly HResult E_INVALIDARG = 0x80070057;

    /// <summary>
    /// No such interface supported
    /// </summary>
    public static readonly HResult E_NOINTERFACE = 0x80004002;

    /// <summary>
    /// Not implemented
    /// </summary>
    public static readonly HResult E_NOTIMPL = 0x80004001;

    /// <summary>
    /// Failed to allocate necessary memory
    /// </summary>
    public static readonly HResult E_OUTOFMEMORY = 0x8007000E;

    /// <summary>
    /// Pointer that is not valid
    /// </summary>
    public static readonly HResult E_POINTER = 0x80004003;

    /// <summary>
    /// Unexpected failure
    /// </summary>
    public static readonly HResult E_UNEXPECTED = 0x8000FFFF;

    /// <summary>
    /// The data necessary to complete this operation is not yet available
    /// </summary>
    public static readonly HResult E_PENDING = 0x8000000A;

    /// <summary>
    /// The operation attempted to access data outside the valid range
    /// </summary>
    public static readonly HResult E_BOUNDS = 0x8000000B;

    /// <summary>
    /// A concurrent or interleaved operation changed the state of the object
    /// </summary>
    public static readonly HResult E_CHANGED_STATE = 0x8000000C;

    /// <summary>
    /// An illegal state change was requested
    /// </summary>
    public static readonly HResult E_ILLEGAL_STATE_CHANGE = 0x8000000D;

    /// <summary>
    /// A method was called at an unexpected time
    /// </summary>
    public static readonly HResult E_ILLEGAL_METHOD_CALL = 0x8000000E;

    /// <summary>
    /// This class cannot be aggregated
    /// </summary>
    public static readonly HResult CLASS_E_NOAGGREGATION = 0x80040110;

    /// <summary>
    /// Class is not available
    /// </summary>
    public static readonly HResult CLASS_E_CLASSNOTAVAILABLE = 0x80040111;

    /// <summary>
    /// Member not found
    /// </summary>
    public static readonly HResult DISP_E_MEMBERNOTFOUND = 0x80020003;

    /// <summary>
    /// Type mismatch
    /// </summary>
    public static readonly HResult DISP_E_TYPEMISMATCH = 0x80020005;

    /// <summary>
    /// Unknown name
    /// </summary>
    public static readonly HResult DISP_E_UNKNOWNNAME = 0x80020006;

    /// <summary>
    /// Exception occurred
    /// </summary>
    public static readonly HResult DISP_E_EXCEPTION = 0x80020009;

    /// <summary>
    /// Out of present range
    /// </summary>
    public static readonly HResult DISP_E_OVERFLOW = 0x8002000A;

    /// <summary>
    /// Invalid index
    /// </summary>
    public static readonly HResult DISP_E_BADINDEX = 0x8002000B;


    [FieldOffset(0)]
    private readonly uint _hresult;

    public bool IsSuccess
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (_hresult & FAIL_MASK) == 0U;
    }

    public bool IsFailure
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => (_hresult & FAIL_MASK) != 0U;
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

    public override string ToString() => ToString(null);
}
