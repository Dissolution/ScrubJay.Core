namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <b>all</b> values
/// </summary>
public static class GenericExtensions
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
    
    public static bool Is<TIn, TOut>(this TIn input, [MaybeNullWhen(false)] out TOut output)
        where TOut : class
    {
        if (input is TOut out2)
        {
            output = out2;
            return true;
        }
        else
        {
            output = default;
            return false;
        }
    }
    
}