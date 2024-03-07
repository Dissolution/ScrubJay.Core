using System.Diagnostics.CodeAnalysis;
using ScrubJay;

#if NET7_0_OR_GREATER

Result result = Testing.TryParse<int>("147", out int value);

Result<int> intResult = Testing.TryParse<int>("147");








#endif
Console.WriteLine("Press enter to close this window");
Console.ReadLine();


static class Testing
{
#if NET7_0_OR_GREATER
    public static Result TryParse<T>(string? str, [MaybeNullWhen(false)] out T? value)
        where T : IParsable<T>
    {
        if (T.TryParse(str, null, out value))
            return true;
        return new ArgumentException($"Cannot parse '{str}' as  {typeof(T).Name} value", nameof(str));
    }

    public static Result<T> TryParse<T>(string? str)
        where T : IParsable<T>
    {
        if (T.TryParse(str, null, out var value))
            return value;
        return new ArgumentException($"Cannot parse '{str}' as  {typeof(T).Name} value", nameof(str));
    }
#endif
}