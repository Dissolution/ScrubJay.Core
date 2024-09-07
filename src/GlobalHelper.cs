global using NotNullAttribute = System.Diagnostics.CodeAnalysis.NotNullAttribute;
global using TextBuffer = ScrubJay.Buffers.Buffer<char>;

namespace ScrubJay;

/// <summary>
/// Global helper methods to support <see cref="Option{T}"/> and <see cref="Result{TOk, TError}"/>
/// </summary>
/// <remarks>
/// To include this in a single <c>.cs</c> file, add the following to the very top:<br/>
/// <c>using static ScrubJay.GlobalHelper;</c><br/>
/// <br/>
/// To include this in an entire project, add the following to that project's <c>.csproj</c> file:<br/>
/// <c>&lt;ItemGroup&gt;</c><br/>
///     <c>&lt;Using Include="ScrubJay.GlobalHelper" Static="true"/&gt;</c><br/>
/// <c>&lt;/ItemGroup&gt;</c><br/>
/// </remarks>
public static class GlobalHelper
{
    /// <summary>
    /// None represents the lack of a value for an <see cref="Option{T}"/> and implicitly converts to <see cref="Option{T}"/>.<see cref="Option{T}.None"/> for any T
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static None None() => default(None);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> None<T>() => Option<T>.None;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Unit Unit() => default(Unit);
    
    /// <inheritdoc cref="Option{T}.Some"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Some<T>(T value) => Option<T>.Some(value);
}