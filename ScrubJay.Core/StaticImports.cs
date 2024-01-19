global using Result = ScrubJay.Result<ScrubJay.Nothing>;

global using static ScrubJay.StaticImports;

namespace ScrubJay;

// ReSharper disable once InconsistentNaming
public static class StaticImports
{
    private static readonly Result _okResult = Result.Ok(default);
    
    /// <inheritdoc cref="Option{T}.None"/>
    public static Option<T> None<T>() => Option<T>.None;
    
    /// <inheritdoc cref="Option{T}.Some"/>
    public static Option<T> Some<T>(T value) => Option<T>.Some(value);

    /// <inheritdoc cref="Result{T}.Ok"/>
    public static Result Ok() => _okResult;

    /// <inheritdoc cref="Result{T}.Error"/>
    public static Exception Error(Exception? error) => error ?? new Exception();

    /// <inheritdoc cref="Result{T}.Ok"/>
    public static Result<TValue> Ok<TValue>(TValue value) => Result<TValue>.Ok(value);
    
    /// <inheritdoc cref="Result{T}.Error"/>
    public static Result<TValue> Error<TValue>(Exception? error) => Result<TValue>.Error(error);
}