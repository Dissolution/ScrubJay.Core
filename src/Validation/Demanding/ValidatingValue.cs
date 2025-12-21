namespace ScrubJay.Validation.Demanding;

/// <summary>
/// Information about a value being validated
/// </summary>
/// <typeparam name="T"></typeparam>
[PublicAPI]
public readonly ref struct ValidatingValue<T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ValidatingValue<T> New(T value,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        return new ValidatingValue<T>(value, valueName);
    }

    /// <summary>
    /// The <typeparamref name="T"/> value being validated
    /// </summary>
    public readonly T Value;

    /// <summary>
    /// An optional captured name for the <see cref="Value"/>
    /// </summary>
    public readonly string? ValueName;

    /// <summary>
    /// The <see cref="Value"/>'s <see cref="Type"/> (may be more specific than <typeparamref name="T"/>)
    /// </summary>
    public Type ValueType
    {
#if NET9_0_OR_GREATER
        get => typeof(T);
#else
        get => Value?.GetType() ?? typeof(T);
#endif
    }

    /// <summary>
    /// The <see cref="string"/> representation of <see cref="Value"/> (the same as calling <see cref="object.ToString()"/> on it)
    /// </summary>
    public string ValueString
    {
#if NET9_0_OR_GREATER
        get => Value.Stringify();
#else
        get => Value?.ToString() ?? string.Empty;
#endif
    }

    internal ValidatingValue(T value, string? valueName)
    {
        Value = value;
        ValueName = valueName;
    }
}