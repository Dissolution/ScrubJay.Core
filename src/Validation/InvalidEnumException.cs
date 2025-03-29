#pragma warning disable CA1715, CA1032

namespace ScrubJay.Validation;

/// <summary>
/// An Exception caused by an undefined <see cref="Enum"/> value
/// </summary>
[PublicAPI]
public class InvalidEnumException : ArgumentOutOfRangeException
{
    private static string GetMessage<E>(E e)
        where E : struct, Enum
        => $"Invalid {typeof(E).NameOf()} enum: {e}";

    public static InvalidEnumException Create<E>(
        E @enum,
        string? message = null,
        [CallerArgumentExpression(nameof(@enum))]
        string? enumName = null)
        where E : struct, Enum
        => new(enumName, (object)@enum, message ?? GetMessage<E>(@enum));

    protected InvalidEnumException(string? enumName, object @enum, string? message)
        : base(enumName, @enum, message)
    {

    }
}
