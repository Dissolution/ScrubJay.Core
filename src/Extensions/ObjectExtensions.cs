// ReSharper disable UseSwitchCasePatternVariable
// ReSharper disable MergeCastWithTypeCheck
namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <see cref="object"/>
/// </summary>
public static class ObjectExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Is<T>(this object? input, [NotNullWhen(true)] out T? output)
    {
        if (input is T)
        {
            output = (T)input;
            return true;
        }
        output = default;
        return false;
    }
    
    public static bool CanBe<T>(this object? input, out T? output)
    {
        switch (input)
        {
            case T:
                output = (T)input;
                return true;
            case null when typeof(T).CanContainNull():
                output = default;
                return true;
            default:
                output = default;
                return false;
        }
    }
}