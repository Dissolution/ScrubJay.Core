// alias `ReadOnlySpan<char>` to `text`
global using text = System.ReadOnlySpan<char>;
// prevent attribute name conflict with `JetBrains.Annotations.NotNullAttribute`
global using NotNullAttribute = System.Diagnostics.CodeAnalysis.NotNullAttribute;

#pragma warning disable CA1715

namespace ScrubJay;

/// <summary>
/// Global helper methods
/// </summary>
/// <remarks>
/// To include these methods in a single <c>.cs</c> file, add to its <c>usings</c> section:<br/>
/// <code>
/// using static ScrubJay.Prelude
/// </code><br/>
/// To include them in an entire project, add to its <c>.csproj</c> file:<br/>
/// <code>
/// &lt;ItemGroup&gt;
///     &lt;Using Include="ScrubJay.Prelude" Static="true"/&gt;
/// &lt;/ItemGroup&gt;
/// </code><br/>
/// </remarks>
[PublicAPI]
public static class Prelude
{
    /// <summary>
    /// <see cref="ScrubJay.Functional.Unit"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Unit Unit() => default(Unit);


    /// <summary>
    /// <see cref="ScrubJay.Functional.None"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static None None() => default(None);

    /// <summary>
    /// <see cref="ScrubJay.Functional.Option{T}"/>.<see cref="ScrubJay.Functional.Option{T}.None"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> None<T>() => Option<T>.None();

    /// <summary>
    /// <see cref="ScrubJay.Functional.Option{T}"/>.<see cref="ScrubJay.Functional.Option{T}.Some"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Some<T>(T value) => Option<T>.Some(value);


    /// <summary>
    /// <see cref="ScrubJay.Functional.Result{T}"/>.<see cref="ScrubJay.Functional.Result{T}.Ok"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Ok<T>(T value)
        => Result<T>.Ok(value);

    /// <summary>
    /// <see cref="ScrubJay.Functional.Result{T,E}"/>.<see cref="ScrubJay.Functional.Result{T,E}.Ok"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, E> Ok<T, E>(T value)
        => Result<T, E>.Ok(value);

    /// <summary>
    /// <see cref="ScrubJay.Functional.Result{T}"/>.<see cref="ScrubJay.Functional.Result{T}.Error"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T> Error<T>(Exception error)
        => Result<T>.Error(error);

    /// <summary>
    /// <see cref="ScrubJay.Functional.Result{T,E}"/>.<see cref="ScrubJay.Functional.Result{T,E}.Error"/>
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<T, E> Error<T, E>(E error)
        => Result<T, E>.Error(error);
}