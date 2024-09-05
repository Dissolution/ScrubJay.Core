// ReSharper disable InconsistentNaming

using ScrubJay.Comparison;
using ScrubJay.Buffers;
using ScrubJay.Text;


namespace ScrubJay;

// <a href="https://doc.rust-lang.org/std/option/index.html">Rust's Option</a>

public readonly ref struct SpanOption<T>
{
#region Operators

    public static implicit operator bool(SpanOption<T> option) => option._isSome;
    public static implicit operator SpanOption<T>(Option.None none) => None;
    public static implicit operator SpanOption<T>(ReadOnlySpan<T> span) => Some(span);

    public static bool operator true(SpanOption<T> option) => option._isSome;
    public static bool operator false(SpanOption<T> option) => !option._isSome;

    public static bool operator ==(SpanOption<T> x, SpanOption<T> y) => x.Equals(y);
    public static bool operator !=(SpanOption<T> x, SpanOption<T> y) => !x.Equals(y);
    public static bool operator >(SpanOption<T> x, SpanOption<T> y) => x.CompareTo(y) > 0;
    public static bool operator >=(SpanOption<T> x, SpanOption<T> y) => x.CompareTo(y) >= 0;
    public static bool operator <(SpanOption<T> x, SpanOption<T> y) => x.CompareTo(y) < 0;
    public static bool operator <=(SpanOption<T> x, SpanOption<T> y) => x.CompareTo(y) <= 0;

    public static bool operator ==(SpanOption<T> option, Option.None none) => option.IsNone();
    public static bool operator !=(SpanOption<T> option, Option.None none) => option.IsSome();
    public static bool operator >(SpanOption<T> option, Option.None none) => option.CompareTo(none) > 0;
    public static bool operator >=(SpanOption<T> option, Option.None none) => option.CompareTo(none) >= 0;
    public static bool operator <(SpanOption<T> option, Option.None none) => option.CompareTo(none) < 0;
    public static bool operator <=(SpanOption<T> option, Option.None none) => option.CompareTo(none) <= 0;

    public static bool operator ==(SpanOption<T> left, ReadOnlySpan<T> right) => left.Equals(right);
    public static bool operator !=(SpanOption<T> left, ReadOnlySpan<T> right) => !left.Equals(right);
    public static bool operator >(SpanOption<T> left, ReadOnlySpan<T> right) => left.CompareTo(right) > 0;
    public static bool operator >=(SpanOption<T> left, ReadOnlySpan<T> right) => left.CompareTo(right) >= 0;
    public static bool operator <(SpanOption<T> left, ReadOnlySpan<T> right) => left.CompareTo(right) < 0;
    public static bool operator <=(SpanOption<T> left, ReadOnlySpan<T> right) => left.CompareTo(right) <= 0;

#endregion

    /// <summary>
    /// Gets the None option
    /// </summary>
    public static SpanOption<T> None => default;

    /// <summary>
    /// Creates a new <see cref="Option{T}"/>.Some containing <paramref name="span"/>
    /// </summary>
    /// <param name="span"></param>
    /// <returns></returns>
    public static SpanOption<T> Some(ReadOnlySpan<T> span) => new(span);


    private readonly bool _isSome;
    private readonly ReadOnlySpan<T> _span;

    private SpanOption(ReadOnlySpan<T> span)
    {
        _isSome = true;
        _span = span;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.is_some"/>
    public bool IsSome() => _isSome;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    /// <a href="https://doc.rust-lang.org/std/option/enum.Option.html#method.is_some_and"/>
    public bool IsSomeAnd(RSpanFunc<T, bool> predicate) => _isSome && predicate(_span!);

    public bool IsSome(out ReadOnlySpan<T> span)
    {
        if (_isSome)
        {
            span = _span!;
            return true;
        }

        span = default;
        return false;
    }

    public bool IsNone() => !_isSome;

    public ReadOnlySpan<T> SomeOrThrow(string? errorMessage = null)
    {
        if (_isSome)
            return _span!;
        throw new InvalidOperationException(errorMessage ?? "This option is not some");
    }

    public ReadOnlySpan<T> SomeOr(ReadOnlySpan<T> span)
    {
        if (_isSome)
            return _span!;
        return span;
    }

    public ReadOnlySpan<T> SomeOrDefault()
    {
        {
            if (_isSome)
                return _span!;
            return default;
        }
    }

    public ReadOnlySpan<T> SomeOrElse(FuncRSpan<T> getSpan)
    {
        if (_isSome)
            return _span!;
        return getSpan();
    }

    public SpanOption<T> Filter(RSpanFunc<T, bool> predicate)
    {
        if (IsSome(out var value) && predicate(value))
            return this;
        return None;
    }

    public SpanOption<U> Map<U>(RSpanFuncRSpan<T, U> map)
    {
        if (IsSome(out var value))
        {
            return SpanOption<U>.Some(map(value));
        }

        return SpanOption<U>.None;
    }

    public Option<U> Map<U>(RSpanFunc<T, U> map)
    {
        if (IsSome(out var value))
        {
            return Option<U>.Some(map(value));
        }

        return Option<U>.None;
    }

    public void Match(RSpanAction<T> onSome, Action onNone)
    {
        if (_isSome)
        {
            onSome(_span!);
        }
        else
        {
            onNone();
        }
    }

    public void Match(RSpanAction<T> onSome, Action<Option.None> onNone)
    {
        if (_isSome)
        {
            onSome(_span!);
        }
        else
        {
            onNone(default);
        }
    }

    public TResult Match<TResult>(RSpanFunc<T, TResult> some, Func<TResult> none)
    {
        if (_isSome)
        {
            return some(_span!);
        }
        else
        {
            return none();
        }
    }

    public TResult Match<TResult>(RSpanFunc<T, TResult> some, Func<Option.None, TResult> none)
    {
        if (_isSome)
        {
            return some(_span!);
        }
        else
        {
            return none(default);
        }
    }

    public ReadOnlySpan<TResult> Match<TResult>(RSpanFuncRSpan<T, TResult> some, FuncRSpan<TResult> none)
    {
        if (_isSome)
        {
            return some(_span!);
        }
        else
        {
            return none();
        }
    }

    public ReadOnlySpan<TResult> Match<TResult>(RSpanFuncRSpan<T, TResult> some, FuncRSpan<Option.None, TResult> none)
    {
        if (_isSome)
        {
            return some(_span!);
        }
        else
        {
            return none(default);
        }
    }

#region Compare

    public int CompareTo(SpanOption<T> spanOption)
    {
        // None compares as less than any Some
        if (_isSome)
        {
            if (spanOption._isSome)
            {
                return Compare.Sequence(_span, spanOption._span);
            }
            else // y is none
            {
                return 1; // I am greater
            }
        }
        else // x is none
        {
            if (spanOption._isSome)
            {
                return -1; // I am lesser
            }
            else
            {
                // None == None
                return 0;
            }
        }
    }

    public int CompareTo(ReadOnlySpan<T> span)
    {
        if (_isSome)
        {
            return Compare.Sequence(_span, span);
        }
        else
        {
            // None compares as less than any Some
            return -1;
        }
    }

    public int CompareTo(IEnumerable<T>? items)
    {
        if (_isSome)
        {
            return Compare.Sequence(_span, items);
        }
        else
        {
            // None compares as less than any Some
            return -1;
        }
    }

    public int CompareTo(Option.None none)
    {
        // None compares as less than any Some
        if (_isSome)
        {
            return 1; // I am greater
        }
        else // x is none
        {
            // None == None
            return 0;
        }
    }

    public int CompareTo(object? obj)
    {
        return obj switch
        {
            T[] itemArray => CompareTo(itemArray.AsSpan()),
            IEnumerable<T> items => CompareTo(items),
            Option.None none => CompareTo(none),
            _ => 1, // unknown values sort before
        };
    }

#endregion

#region Equals

    public bool Equals(SpanOption<T> spanOption)
    {
        if (_isSome)
        {
            if (spanOption._isSome)
            {
                return Equate.Sequence(_span, spanOption._span);
            }
            else // y is none
            {
                return false;
            }
        }
        else // x is none
        {
            if (_isSome)
            {
                return false;
            }
            else
            {
                // None == None
                return true;
            }
        }
    }

    public bool Equals(ReadOnlySpan<T> span)
    {
        return _isSome && Equate.Sequence(_span, span);
    }

    public bool Equals(IEnumerable<T>? items)
    {
        return _isSome && Equate.Sequence(_span, items);
    }

    public bool Equals(Option.None none) => !_isSome;

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return obj switch
        {
            T[] array => Equals(array.AsSpan()),
            IEnumerable<T> items => Equals(items),
            Option.None none => Equals(none),
            _ => false,
        };
    }

#endregion

    public override int GetHashCode()
    {
        if (_isSome)
            return Hasher.Combine(_span);
        return 0;
    }

    public override string ToString()
    {
        if (_isSome)
        {
            var text = new Buffer<char>();
            text.Append("Some(");
            if (_span.Length > 0)
            {
                text.AppendFormatted<T>(_span[0]);
                for (var i = 1; i < _span.Length; i++)
                {
                    text.Append(", ");
                    text.AppendFormatted<T>(_span[i]);
                }
            }
            text.Append(')');
            return text.ToStringAndDispose();
        }

        return nameof(None);
    }
    
    public SpanOptionEnumerator GetEnumerator() => new SpanOptionEnumerator(this);

    [MustDisposeResource(false)]
    public ref struct SpanOptionEnumerator
    {
        private bool _yielded;
        private readonly ReadOnlySpan<T> _span;
        
        public ReadOnlySpan<T> Current => _span;

        public SpanOptionEnumerator(SpanOption<T> option)
        {
            if (option._isSome)
            {
                _span = option._span!;
                _yielded = false;
            }
            else
            {
                _span = default!;
                _yielded = true;
            }
        }

        public bool MoveNext()
        {
            if (_yielded)
                return false;
            _yielded = true;
            return true;
        }
    }
}