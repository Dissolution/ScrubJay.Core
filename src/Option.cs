namespace ScrubJay;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// Add the following at the top of a <c>.cs</c> file to include <see cref="Option{T}"/> support in that file:<br/>
/// <c>using static ScrubJay.Option;</c><br/>
/// Add the following in a <c>.csproj</c> <c>&lt;ItemGroup/&gt;</c> to include <see cref="Option{T}"/> support in that project:<br/>
/// <c>&lt;Using Include="ScrubJay.Option" Static="true"/&gt;</c><br/>
/// </remarks>
public static class Option
{
    public static readonly None None = default(None);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Some<T>(T value) => Option<T>.Some(value);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> NotNull<T>(T? value)
        where T : notnull
    {
        if (value is not null)
            return Option<T>.Some(value);
        return None;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> From<T>(T? nullable)
        where T : struct
    {
        if (nullable.HasValue)
            return Option<T>.Some(nullable.Value);
        return None;
    }

//    [MethodImpl(MethodImplOptions.AggressiveInlining)]
//    public static Option<T> From<T>(Result<T> result)
//    {
//        if (result.IsOk(out var value))
//            return Some<T>(value);
//        return None;
//    }
}