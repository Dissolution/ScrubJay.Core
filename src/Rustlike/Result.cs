#pragma warning disable

using ScrubJay.Rustlike.mitm;

// ReSharper disable once CheckNamespace
namespace ScrubJay.Rustlike;

[StructLayout(LayoutKind.Auto)]
public ref struct Result<T, E>
    where T : allows ref struct
    where E : allows ref struct
{
    public static implicit operator Result<T, E>(Ok<T> ok) => new(true, ok.Value, default);
    public static implicit operator Result<T, E>(Err<E> err) => new(false, default, err.Value);

    public static Result<T, E> Ok(T value)
    {
        return new(true, value, default);
    }

    public static Result<T, E> Err(E error)
    {
        return new(false, default, error);
    }


    private readonly bool _isOk;
    private readonly T? _value;
    private readonly E? _error;

    private Result(bool isOk, T? value, E? error)
    {
        Debug.Assert(isOk ? error is null : value is null);
        _isOk = isOk;
        _value = value;
        _error = error;
    }

    /// <summary>
    /// Returns <c>true</c> if the Result is <see cref="Ok"/>
    /// </summary>
    /// <seealso href="https://doc.rust-lang.org/std/result/enum.Result.html#method.is_ok"/>
    public bool IsOk() => _isOk;

    /// <summary>
    /// Returns <c>true</c> if the Result is <see cref="Ok"/> and the value inside of it matches a predicate
    /// </summary>
    /// <seealso href="https://doc.rust-lang.org/std/result/enum.Result.html#method.is_ok_and"/>
    public bool IsOkAnd(Fun<T, bool> predicate) => _isOk && predicate(_value!);

    /// <summary>
    /// Converts from <see cref="Result{T,E}"/> to <see cref="Option{T}"/>
    /// </summary>
    /// <seealso href="https://doc.rust-lang.org/std/result/enum.Result.html#method.ok"/>
    public Option<T> Ok()
    {
        if (_isOk)
            return Option<T>.Some(_value!);
        return Option<T>.None;
    }

    /// <summary>
    /// Returns <c>true</c> if the Result is <see cref="Err"/>
    /// </summary>
    /// <seealso href="https://doc.rust-lang.org/std/result/enum.Result.html#method.is_err"/>
    public bool IsErr() => !_isOk;

    /// <summary>
    /// Returns <c>true</c> if the Result is <see cref="Err"/> and the value inside of it matches a predicate
    /// </summary>
    /// <seealso href="https://doc.rust-lang.org/std/result/enum.Result.html#method.is_err_and"/>
    public bool IsErrAnd(Fun<E, bool> predicate) => !_isOk && predicate(_error!);

    /// <summary>
    /// Converts from <see cref="Result{T,E}"/> to <see cref="Option{E}"/>
    /// </summary>
    /// <seealso href="https://doc.rust-lang.org/std/result/enum.Result.html#method.err"/>
    public Option<E> Err()
    {
        if (!_isOk)
            return Option<E>.Some(_error!);
        return Option<E>.None;
    }
}
