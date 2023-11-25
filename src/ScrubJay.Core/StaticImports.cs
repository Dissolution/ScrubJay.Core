namespace ScrubJay;

// ReSharper disable once InconsistentNaming
public static class StaticImports
{
    /// <inheritdoc cref="Option{T}.None"/>
    public static Option<T> None<T>() => Option<T>.None;
    /// <inheritdoc cref="Option{T}.Some"/>
    public static Option<T> Some<T>(T value) => Option<T>.Some(value);
    
    /// <inheritdoc cref="Result.Ok"/>
    public static Result Ok() => Result.Ok();
    /// <inheritdoc cref="Result.Error"/>
    public static Result Error(Exception? error) => Result.Error(error);

    /// <inheritdoc cref="Result{T}.Ok"/>
    public static Result<TValue> Ok<TValue>(TValue value) => Result<TValue>.Ok(value);
    /// <inheritdoc cref="Result{T}.Error"/>
    public static Result<TValue> Error<TValue>(Exception? error) => Result<TValue>.Error(error);
    /// <inheritdoc cref="Result{T}.NotNull"/>
    public static Result<TValue> NotNull<TValue>(TValue? value) => Result<TValue>.NotNull(value);
    
    /// <inheritdoc cref="Result{V,E}.Ok"/>
    public static Result<TOk, TError> Ok<TOk, TError>(TOk okValue) => Result<TOk, TError>.Ok(okValue);
    /// <inheritdoc cref="Result{V,E}.Error"/>
    public static Result<TOk, TError> Error<TOk, TError>(TError? errorValue) => Result<TOk, TError>.Error(errorValue);
}