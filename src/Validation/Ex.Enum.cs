namespace ScrubJay.Validation;

partial class Ex
{
    public static ArgumentOutOfRangeException UndefinedEnum<E>(
        E @enum,
        [CallerArgumentExpression(nameof(@enum))]
        string? enumName = null)
        where E : struct, Enum
    {
        var message = Build($"Enum \"{enumName}\" ({typeof(E):@}) `{@enum}` is undefined");
        return new ArgumentOutOfRangeException(enumName, @enum, message);
    }
}