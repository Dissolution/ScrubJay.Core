using ScrubJay.Functional.IMPL;

namespace ScrubJay.Functional;

[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly ref struct RefOption<T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
#region Operators

    /// <summary>
    /// Implicitly convert an <see cref="Option{T}"/> into <c>true</c> if it is Some and <c>false</c> if it is None
    /// </summary>
    public static implicit operator bool(in RefOption<T> option) => option._isSome;

    /// <summary>
    /// Implicitly convert an <see cref="Option{T}"/> into <c>true</c> if it is Some and <c>false</c> if it is None
    /// </summary>
    public static bool operator true(RefOption<T> option) => option._isSome;

    /// <summary>
    /// Implicitly convert an <see cref="Option{T}"/> into <c>true</c> if it is Some and <c>false</c> if it is None
    /// </summary>
    public static bool operator false(RefOption<T> option) => !option._isSome;

    /// <summary>
    /// Implicitly convert a standalone <see cref="None"/> to an <see cref="Option{T}"/>.<see cref="Option{T}.None"/>
    /// </summary>
    public static implicit operator RefOption<T>(None _) => None;

#endregion

    public static RefOption<T> None => default;

    public static RefOption<T> Some(T value) => new(value);

    // Is this Option.Some?
    // if someone does default(RefOption), this will be false, so default(RefOption) == None
    private readonly bool _isSome;

    // If this is Option.Some, the value
    private readonly T? _value;

    // option can only be constructed with None(), Some(), or implicitly
    private RefOption(T value)
    {
        _isSome = true;
        _value = value;
    }

    public bool IsNone() => !_isSome;

#region Some-ness

    public bool IsSome() => _isSome;

    /// <summary>
    /// Does this <see cref="Option{T}"/> contain <see cref="Some"/> value?
    /// </summary>
    /// <param name="value">
    /// If this is <see cref="Some"/>, this will be the contained value<br/>
    /// if this is <see cref="None"/>, it will be <c>default(T)</c>
    /// </param>
    /// <returns>
    /// <c>true</c> and fills <paramref name="value"/> if this is <see cref="Some"/><br/>
    /// <c>false</c> if it is <see cref="None"/>
    /// </returns>
    public bool IsSome([MaybeNullWhen(false)] out T value)
    {
        if (_isSome)
        {
            value = _value!;
            return true;
        }

        value = default;
        return false;
    }


    public bool IsSomeAnd(Func<T, bool> predicate) => _isSome && predicate(_value!);

    public T SomeOrThrow(string? errorMessage = null)
    {
        if (_isSome)
            return _value!;
        throw Ex.Invalid(errorMessage ?? $"Option<{typeof(T)}> is None");
    }

    public T SomeOr(T fallback)
    {
        if (_isSome)
            return _value!;
        return fallback;
    }

    public T SomeOr(Func<T> getFallback)
    {
        if (_isSome)
            return _value!;
        return getFallback();
    }

    public T? SomeOrDefault()
    {
        if (_isSome)
            return _value!;
        return default;
    }

#endregion

#region Match

    public void Match(Action<T> onSome, Action onNone)
    {
        if (_isSome)
        {
            onSome(_value!);
        }
        else
        {
            onNone();
        }
    }


    public void Match(Action<T> onSome, Action<None> onNone)
    {
        if (_isSome)
        {
            onSome(_value!);
        }
        else
        {
            onNone(default);
        }
    }


    public R Match<R>(Func<T, R> some, Func<R> none)
    {
        if (_isSome)
        {
            return some(_value!);
        }
        else
        {
            return none();
        }
    }


    public R Match<R>(Func<T, R> some, Func<None, R> none)
    {
        if (_isSome)
        {
            return some(_value!);
        }
        else
        {
            return none(default);
        }
    }

#endregion

#region LINQ + IEnumerable

    public RefOption<N> Select<N>(Func<T, N> selector)
    {
        if (_isSome)
            return RefOption<N>.Some(selector(_value!));
        return RefOption<N>.None;
    }

#if NET9_0_OR_GREATER
    public RefOption<N> SelectMany<N>(Func<T, RefOption<N>> newSelector)
    {
        if (_isSome)
        {
            return newSelector(_value!);
        }

        return RefOption<N>.None;
    }
#endif

    public RefOption<N> SelectMany<K, N>(
        Func<T, K> keySelector,
        Func<T, K, N> newSelector)
    {
        if (_isSome)
        {
            var key = keySelector(_value!);
            var newValue = newSelector(_value!, key);
            return RefOption<N>.Some(newValue);
        }

        return RefOption<N>.None;
    }

#if NET9_0_OR_GREATER
    public RefOption<N> SelectMany<K, N>(
        Func<T, RefOption<K>> keySelector,
        Func<T, K, N> newSelector)
    {
        if (_isSome)
        {
            var key = keySelector(_value!);
            if (key.IsSome(out var k))
            {
                var newValue = newSelector(_value!, k);
                return RefOption<N>.Some(newValue);
            }
        }

        return RefOption<N>.None;
    }
#endif

    /// <summary>
    /// Returns <see cref="None"/> if this <see cref="Option{T}"/> is <see cref="None"/>,<br/>
    /// otherwise calls <paramref name="predicate"/> with the wrapped value and returns:<br/>
    /// <see cref="Some"/> if <paramref name="predicate"/> returns <c>true</c> (with the wrapped value),<br/>
    /// and <see cref="None"/> if <paramref name="predicate"/> returns <c>false</c><br/>
    /// This function works similar to <c>Enumerable.Where</c><br/>
    /// You can imagine this <see cref="Option{T}"/> being an iterator over one or zero elements<br/>
    /// <see cref="Where"/> lets you decide which elements to keep<br/>
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    /// <seealso href="https://doc.rust-lang.org/std/option/enum.Option.html#method.filter"/>
    public RefOption<T> Where(Func<T, bool> predicate)
    {
        if (_isSome)
        {
            if (predicate(_value!))
            {
                return this;
            }
        }

        return None;
    }

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    [MustDisposeResource(false)]
    public OptionEnumerator GetEnumerator() => new OptionEnumerator(this);

    [PublicAPI]
    [MustDisposeResource(false)]
    public ref struct OptionEnumerator
    {
        private readonly RefOption<T> _option;
        private bool _canYield;

        public readonly T Current => _option.SomeOrThrow();

        public OptionEnumerator(RefOption<T> option)
        {
            _option = option;
            _canYield = option._isSome;
        }

        public bool MoveNext()
        {
            if (!_canYield)
            {
                return false;
            }
            else
            {
                _canYield = false;
                return true;
            }
        }

        public void Reset()
        {
            _canYield = _option._isSome;
        }
    }

#endregion

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return false;
    }

    public override int GetHashCode()
    {
        return 0;
    }

    public override string ToString()
    {
        if (_isSome)
        {
            return Build($"RefOption<{typeof(T):@}>.Some({_value})");
        }

        return Build($"RefOption<{typeof(T):@}>.None");
    }
}