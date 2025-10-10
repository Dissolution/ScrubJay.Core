namespace ScrubJay.Memory;

/// <summary>
/// Indicates what kind of prefix or postfix that will be used to determine the length of the resulting string
/// </summary>
[PublicAPI]
public enum StringFix
{
    None,
    SevenBitEncodedLenPrefix,
    U8Prefix,
    U16Prefix,
    U32Prefix,
    U64Prefix,
    I8Prefix,
    I16Prefix,
    I32Prefix,
    I64Prefix,
    NullTerminated,
}

/// <summary>
/// Indicates a possible Prefix or Postfix associated with a sequence of bytes
/// that contains an offset from <c>0001-01-01 00:00:00.0</c>
/// </summary>
/// <seealso href="https://en.cppreference.com/w/c/chrono/time_t"/>
[PublicAPI]
public enum TimeFix
{
    /// <summary>
    /// 100-nanosecond intervals (ticks) from origin
    /// </summary>
    /// <remarks>
    /// Size = 8 bytes
    /// </remarks>
    Ticks,

    /// <summary>
    /// <see cref="uint"/> seconds from origin
    /// </summary>
    /// <remarks>
    /// Size = 4 bytes
    /// </remarks>
    TimeU32,

    /// <summary>
    /// <see cref="ulong"/> seconds from origin
    /// </summary>
    /// <remarks>
    /// Size = 8 bytes
    /// </remarks>
    TimeU64,
}

[PublicAPI]
public static class TimeFixExtensions
{
    private static readonly DateTime _dateOrigin = new DateTime(1970, 1, 1);

    extension(TimeFix)
    {
        public static DateTime OriginDateTime
            => _dateOrigin;
    }
}