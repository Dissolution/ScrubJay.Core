namespace ScrubJay.Extensions;

/// <summary>
/// Extensions on <b>all</b> values
/// </summary>
public static class GenericExtensions
{
    public static Option<TOut> As<TIn, TOut>(this TIn input)
        where TOut : class
    {
        if (input is TOut out2)
        {
            return Some(out2);
        }

        return None<TOut>();
    }
    
}