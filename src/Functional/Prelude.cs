global using text = System.ReadOnlySpan<char>;
global using NotNull = System.Diagnostics.CodeAnalysis.NotNullAttribute;

using ScrubJay.Functional.Compat;

namespace ScrubJay.Functional;

// <summary>
// Global helper methods to support <see cref="Option{T}"/> and <see cref="Result{TOk, TError}"/>
// </summary>
// <remarks>
// To make these methods available everywhere in a single <c>.cs</c> file,
// add the following line in the <c>usings</c> section:
// <code>
// using static ScrubJay.GlobalHelper;
// </code>
// To make them available for an entire project,
// add the following lines to that project's <c>.csproj</c> file:
// <code>
// &lt;ItemGroup&gt;
//     &lt;Using Include="ScrubJay.GlobalHelper" Static="true"/&gt;
// &lt;/ItemGroup&gt;
// </code>
// </remarks>
[PublicAPI]
public static class Prelude
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Unit Unit() => default(Unit);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static None None() => default(None);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> None<T>() => Option<T>.None();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Option<T> Some<T>(T value) => Option<T>.Some(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Ok<T> Ok<T>(T value) => new Ok<T>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Result<TOk, TError> Ok<TOk, TError>(TOk ok)
        => Result<TOk, TError>.Ok(ok);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Error<T> Error<T>(T value) => new Error<T>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Error<Exception> Error(Exception exception)
        => new Error<Exception>(exception);

    public static Result<TOk, TError> Error<TOk, TError>(TError error)
        => Result<TOk, TError>.Error(error);
}
