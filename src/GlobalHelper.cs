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
    
    /// <summary>
    /// Converts an <see cref="Exception"/> to an <see cref="Error{T}">Error&lt;Exception&gt;</see>
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    /// <remarks>
    /// Another way to solve the issue:<br/>
    /// <c>Result&lt;?, Exception&gt; result;</c><br/>
    /// You can just use the implicit conversion for inherited types:<br/>
    /// <c>result = new InvalidOperationException();</c><br/>
    /// But if you want to use Error() (as you might be using to solve interface casting issues)<br/>
    /// <c>result = Error(new InvalidOperationException);</c><br/>
    /// This will not work, as Error&lt;InvalidOperationException&gt; does not implicitly cast to Result&lt;?, Exception&gt;<br/>
    /// This overload solves the problem by performing the casting explicitly<br/>
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Error<Exception> Error(Exception exception)
        => new Error<Exception>(exception);
}