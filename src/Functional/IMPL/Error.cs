#pragma warning disable CA1716

namespace ScrubJay.Functional.IMPL;

/// <summary>
/// Represents the <b>Error</b> portion of a <see cref="Result"/>, <see cref="Result{T}"/>, or <see cref="Result{T,E}"/>
/// </summary>
/// <remarks>
/// This class is primarily used for implicit coercions
/// </remarks>
[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly ref struct Error<E>
#if NET9_0_OR_GREATER
    where E : allows ref struct
#endif
{
    public readonly E Value;

    public Error(E value)
    {
        Value = value;
    }

    public void Deconstruct(out E error)
    {
        error = Value;
    }

    public override string ToString()
    {
        return $"Error({Value.Stringify()})";
    }
}