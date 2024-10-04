global using NotNullAttribute = System.Diagnostics.CodeAnalysis.NotNullAttribute;
global using FormatWriter = ScrubJay.Memory.SpanWriter<char>;

namespace ScrubJay;

/// <summary>
/// Global helper methods to support <see cref="Option{T}"/> and <see cref="Result{TOk, TError}"/>
/// </summary>
/// <remarks>
/// To make these methods available everywhere in a single <c>.cs</c> file,
/// add the following line in the <c>usings</c> section:
/// <code>
/// using static ScrubJay.GlobalHelper;
/// </code>
/// To make them available for an entire project,
/// add the following lines to that project's <c>.csproj</c> file:
/// <code>
/// &lt;ItemGroup&gt;
///     &lt;Using Include="ScrubJay.GlobalHelper" Static="true"/&gt;
/// &lt;/ItemGroup&gt;
/// </code>
/// </remarks>
public static class GlobalHelper
{
    /// <summary>
    /// None represents the lack of a value for an <see cref="Option{T}"/> and implicitly converts to <see cref="Option{T}"/>.<see cref="Option{T}.None"/> for any T
    /// </summary>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static None None() => Functional.None.Default;

    /// <inheritdoc cref="Option{T}.None"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> None<T>() => Option<T>.None();
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Unit Unit() => Functional.Unit.Default;
    
    /// <inheritdoc cref="Option{T}.Some"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Some<T>(T value) => Option<T>.Some(value);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Ok<Unit> Ok() => new Ok<Unit>(Functional.Unit.Default);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Ok<T> Ok<T>(T value) => new Ok<T>(value);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Error<T> Error<T>(T value) => new Error<T>(value);
    
    /* Another way to solve the issue:
     * Result<?, Exception> result;
     * 
     * You can just use the implicit conversion for inherited types:
     * result = new InvalidOperationException();
     *
     * But if you want to use Error() (as you might be using to solve interface casting issues)
     * result = Error(new InvalidOperationException);
     * This will not work, as Error<InvalidOperationException> does not implicitly cast to Result<?, Exception>
     *
     * The below overload solves the problem by performing the casting itself
     */
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Error<Exception> Error(Exception exception)
        => new Error<Exception>(exception);
}