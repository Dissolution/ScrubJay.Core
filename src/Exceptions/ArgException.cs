#pragma warning disable CA1032 // Implement standard exception constructors

namespace ScrubJay.Exceptions;

[PublicAPI]
public class ArgException : Exception
{
    internal static string GetMessage(
        object? argument,
        Type? argumentType,
        string? argumentName,
        InterpolatedText interpolatedInfo) => TextBuilder
        .New
        .Append($"Invalid {argumentType:@} argument \"{argumentName}\" - `{argument:@}`")
        .IfNotEmpty(interpolatedInfo, static (tb,info) => tb.Append(": ").Append(info))
        .ToStringAndDispose();

    public static ArgException New(
        object? argument,
        InterpolatedText interpolatedInfo,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        return new ArgException(argument, argument?.GetType(), argumentName, interpolatedInfo, innerException);
    }

    public static ArgException New(
        object? argument,
        Type? argumentType,
        InterpolatedText interpolatedInfo,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        return new ArgException(argument, argumentType, argumentName, interpolatedInfo, innerException);
    }


    public static ArgException New<T>(
        object? argument,
        InterpolatedText interpolatedInfo,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        return new ArgException(argument, typeof(T), argumentName, interpolatedInfo, innerException);
    }

    public static ArgException New<T>(
        T? argument,
        InterpolatedText interpolatedInfo,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        return new ArgException(argument, typeof(T), argumentName, interpolatedInfo, innerException);
    }


    public object? ArgumentObject { get; }

    public Type? ArgumentType { get; }

    public string? ArgumentName { get; }



    private ArgException(object? argument, Type? argumentType, string? argumentName,
        InterpolatedText interpolatedInfo, Exception? innerException = null)
        : base(GetMessage(argument, argumentType, argumentName, interpolatedInfo), innerException)
    {
        this.ArgumentObject = argument;
        this.ArgumentType = argumentType;
        this.ArgumentName = argumentName;
    }
}