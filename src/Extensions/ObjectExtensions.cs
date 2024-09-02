// ReSharper disable UseSwitchCasePatternVariable
// ReSharper disable MergeCastWithTypeCheck
namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <see cref="object"/>
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Tests if this <see cref="object"/> is a valid <typeparamref name="T"/> value
    /// </summary>
    /// <param name="input">The <see cref="object"/> to validate</param>
    /// <param name="output">
    /// If the <paramref name="input"/> <see cref="object"/> is a valid <typeparamref name="T"/> value, that value<br/>
    /// otherwise, <c>default(</c><typeparamref name="T"/><c>)</c>
    /// </param>
    /// <typeparam name="T">
    /// The <see cref="Type"/> of value to check for
    /// </typeparam>
    /// <returns>
    /// <c>true</c> and non-<c>null</c> <paramref name="output"/> if <paramref name="input"/> is a <typeparamref name="T"/><br/>
    /// <c>false</c> and <c>default</c> <paramref name="output"/> otherwise
    /// </returns>
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
    
    /// <summary>
    /// Tests if this <see cref="object"/> can be a <typeparamref name="T"/> value,<br/>
    /// which includes allowing <c>null</c> for <c>class</c> and <c>interface</c> types
    /// </summary>
    /// <param name="input"></param>
    /// <param name="output"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
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