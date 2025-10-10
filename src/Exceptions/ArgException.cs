#pragma warning disable CA1032 // Implement standard exception constructors

namespace ScrubJay.Exceptions;

[PublicAPI]
public class ArgException : Exception
{
    internal static string GetMessage(object? argument, Type? argumentType, string? argumentName, string? info) =>
        TextBuilder
            .New
            .Append("Invalid ")
            .Render(argumentType)
            .Append(" \"")
            .Append(argumentName)
            .Append("\" `")
            .Render(argument)
            .Append('`')
            .IfNotEmpty(info, static (tb, n) => tb.Append(": ").Write(n))
            .ToStringAndDispose();

    internal static string GetMessage(object? argument, Type? argumentType, string? argumentName,
        ref InterpolatedTextBuilder interpolatedInfo) =>
        TextBuilder
            .New
            .Append("Invalid ")
            .Render(argumentType)
            .Append(" \"")
            .Append(argumentName)
            .Append("\" `")
            .Render(argument)
            .Append("`: ")
            .Append(ref interpolatedInfo)
            .ToStringAndDispose();


    public static ArgException New(object? argument,
        string? info = null,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        return new ArgException(argument, argument?.GetType(), argumentName, info, innerException);
    }

    public static ArgException New(object? argument,
        ref InterpolatedTextBuilder interpolatedInfo,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        return new ArgException(argument, argument?.GetType(), argumentName, ref interpolatedInfo, innerException);
    }

    public static ArgException New(object? argument,
        Type? argumentType,
        string? info = null,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        return new ArgException(argument, argumentType, argumentName, info, innerException);
    }

    public static ArgException New(object? argument,
        Type? argumentType,
        ref InterpolatedTextBuilder interpolatedInfo,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        return new ArgException(argument, argumentType, argumentName, ref interpolatedInfo, innerException);
    }


    public static ArgException New<T>(object? argument,
        string? info = null,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        return new ArgException(argument, typeof(T), argumentName, info, innerException);
    }

    public static ArgException New<T>(object? argument,
        ref InterpolatedTextBuilder interpolatedInfo,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        return new ArgException(argument, typeof(T), argumentName, ref interpolatedInfo, innerException);
    }



    public static ArgException New<T>(T? argument,
        string? info = null,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        return new ArgException(argument, typeof(T), argumentName, info, innerException);
    }

    public static ArgException New<T>(T? argument,
        ref InterpolatedTextBuilder interpolatedInfo,
        Exception? innerException = null,
        [CallerArgumentExpression(nameof(argument))]
        string? argumentName = null)
    {
        return new ArgException(argument, typeof(T), argumentName, ref interpolatedInfo, innerException);
    }


    public object? ArgumentObject { get; }

    public Type? ArgumentType { get; }

    public string? ArgumentName { get; }

    private ArgException(object? argument, Type? argumentType, string? argumentName, string? info,
        Exception? innerException = null)
        : base(GetMessage(argument, argumentType, argumentName, info), innerException)
    {
        this.ArgumentObject = argument;
        this.ArgumentType = argumentType;
        this.ArgumentName = argumentName;
    }

    private ArgException(object? argument, Type? argumentType, string? argumentName,
        ref InterpolatedTextBuilder interpolatedInfo, Exception? innerException = null)
        : base(GetMessage(argument, argumentType, argumentName, ref interpolatedInfo), innerException)
    {
        this.ArgumentObject = argument;
        this.ArgumentType = argumentType;
        this.ArgumentName = argumentName;
    }
}