namespace ScrubJay.Functional;

[PublicAPI]
[StructLayout(LayoutKind.Auto)]
public readonly struct Either<TLeft, TRight> :
/* All listed interfaces are implemented, but cannot be declared because they may unify for some type parameter substitutions */
#if NET7_0_OR_GREATER
    IEqualityOperators<Either<TLeft, TRight>, Either<TLeft, TRight>, bool>,
    //IEqualityOperators<Either<TLeft, TRight>, TLeft, bool>,
    //IEqualityOperators<Either<TLeft, TRight>, R, bool>,
#endif
    IEquatable<Either<TLeft, TRight>>
    //IEquatable<TLeft>,
    //IEquatable<TRight>
{
#region Operators
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Either<TLeft, TRight>(TLeft value) => Left(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Either<TLeft, TRight>(TRight value) => Right(value);

    public static bool operator ==(Either<TLeft, TRight> left, Either<TLeft, TRight> right) => left.Equals(right);
    public static bool operator !=(Either<TLeft, TRight> left, Either<TLeft, TRight> right) => !left.Equals(right);
    public static bool operator ==(Either<TLeft, TRight> either, TLeft? left) => either.Equals(left);
    public static bool operator !=(Either<TLeft, TRight> either, TLeft? left) => !either.Equals(left);
    public static bool operator ==(Either<TLeft, TRight> either, TRight? right) => either.Equals(right);
    public static bool operator !=(Either<TLeft, TRight> either, TRight? right) => !either.Equals(right);

#endregion
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Either<TLeft, TRight> Left(TLeft left) => new Either<TLeft, TRight>(true, left, default);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Either<TLeft, TRight> Right(TRight right) => new Either<TLeft, TRight>(false, default, right);



    private readonly bool _isLeft;
    private readonly TLeft? _left;
    private readonly TRight? _right;

    public bool IsLeft
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _isLeft;
    }

    public bool IsRight
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => !_isLeft;
    }

    private Either(bool isLeft, TLeft? left, TRight? right)
    {
        _isLeft = isLeft;
        _left = left;
        _right = right;
    }



    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasLeft([MaybeNullWhen(false)] out TLeft left)
    {
        if (_isLeft)
        {
            left = _left!;
            return true;
        }

        left = default!;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasRight([MaybeNullWhen(false)] out TRight right)
    {
        if (!_isLeft)
        {
            right = _right!;
            return true;
        }

        right = default!;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsLeftAnd(Func<TLeft, bool> leftPredicate) => _isLeft && leftPredicate(_left!);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsRightAnd(Func<TRight, bool> rightPredicate) => !_isLeft && rightPredicate(_right!);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TLeft LeftOrThrow(string? exceptionMessage = null)
    {
        if (_isLeft)
            return _left!;
        throw (_right as Exception) ?? new InvalidOperationException(exceptionMessage ?? "This Either is not Left");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TRight RightOrThrow(string? exceptionMessage = null)
    {
        if (!_isLeft)
            return _right!;
        throw (_left as Exception) ?? new InvalidOperationException(exceptionMessage ?? "This Either is not Right");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TLeft LeftOr(TLeft fallbackLeft)
    {
        if (_isLeft)
            return _left!;
        return fallbackLeft;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TRight RightOr(TRight right)
    {
        if (!_isLeft)
            return _right!;
        return right;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TLeft? LeftOrDefault()
    {
        if (_isLeft)
            return _left!;
        return default(TLeft);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TRight? RightOrDefault()
    {
        if (!_isLeft)
            return _right!;
        return default(TRight);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TLeft LeftOrElse(Func<TLeft> getLeft)
    {
        if (_isLeft)
            return _left!;
        return getLeft();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TRight RightOrElse(Func<TRight> getRight)
    {
        if (_isLeft)
            return _right!;
        return getRight();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasLeftOrRight([MaybeNullWhen(false)] out TLeft left, [MaybeNullWhen(true)] out TRight right)
    {
        if (_isLeft)
        {
            left = _left!;
            right = _right;
            return true;
        }

        left = _left;
        right = _right!;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasRightOrLeft([MaybeNullWhen(false)] out TRight right, [MaybeNullWhen(true)] out TLeft left)
    {
        if (!_isLeft)
        {
            right = _right!;
            left = _left;
            return true;
        }

        right = _right;
        left = _left!;
        return false;
    }

    public Either<TNewLeft, TRight> SelectLeft<TNewLeft>(Func<TLeft, TNewLeft> selector)
    {
        if (_isLeft)
        {
            return Either<TNewLeft, TRight>.Left(selector(_left!));
        }

        return Either<TNewLeft, TRight>.Right(_right!);
    }

    public Either<TNewLeft, TRight> SelectLeft<TNewLeft>(Func<TLeft, Either<TNewLeft, TRight>> selector)
    {
        if (_isLeft)
            return selector(_left!);
        return _right!;
    }

    public Either<TLeft, TNewRight> SelectRight<TNewRight>(Func<TRight, TNewRight> selector)
    {
        if (_isLeft)
        {
            return Either<TLeft, TNewRight>.Left(_left!);
        }

        return Either<TLeft, TNewRight>.Right(selector(_right!));
    }

    public Either<TLeft, TNewRight> SelectRight<TNewRight>(Func<TRight, Either<TLeft, TNewRight>> selector)
    {
        if (!_isLeft)
            return selector(_right!);
        return _left!;
    }


    public TNewLeft SelectLeftOr<TNewLeft>(Func<TLeft, TNewLeft> selector, TNewLeft defaultLeft)
    {
        if (_isLeft)
        {
            return selector(_left!);
        }
        return defaultLeft;
    }

    public TNewRight SelectRightOr<TNewRight>(Func<TRight, TNewRight> selector, TNewRight defaultRight)
    {
        if (!_isLeft)
        {
            return selector(_right!);
        }
        return defaultRight;
    }

    public TNewLeft SelectLeftOrElse<TNewLeft>(Func<TLeft, TNewLeft> selector, Func<TNewLeft> getLeft)
    {
        if (_isLeft)
            return selector(_left!);

        return getLeft();
    }

    public TNewRight SelectRightOrElse<TNewRight>(Func<TRight, TNewRight> selector, Func<TNewRight> getRight)
    {
        if (!_isLeft)
            return selector(_right!);

        return getRight();
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Match(Action<TLeft> onLeft, Action<TRight> onRight)
    {
        if (_isLeft)
        {
            onLeft(_left!);
        }
        else
        {
            onRight(_right!);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TEither Match<TEither>(Func<TLeft, TEither> onLeft, Func<TRight, TEither> onRight)
    {
        if (_isLeft)
        {
            return onLeft(_left!);
        }
        else
        {
            return onRight(_right!);
        }
    }


#region Equal

    public bool Equals(Either<TLeft, TRight> either)
    {
        if (_isLeft)
        {
            if (either._isLeft)
            {
                return EqualityComparer<TLeft>.Default.Equals(_left!, either._left!);
            }

            return false;
        }

        if (either._isLeft)
        {
            return false;
        }

        return EqualityComparer<TRight>.Default.Equals(_right!, either._right!);
    }

    public bool Equals(TLeft? left)
    {
        if (_isLeft)
        {
            return EqualityComparer<TLeft>.Default.Equals(_left!, left!);
        }

        return false;
    }

    public bool Equals(TRight? right)
    {
        if (!_isLeft)
        {
            return EqualityComparer<TRight>.Default.Equals(_right!, right!);
        }

        return false;
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
        => obj switch
        {
            Either<TRight, TRight> either => Equals(either),
            TLeft left => Equals(left),
            TRight right => Equals(right),
            bool isLeft => _isLeft == isLeft,
            _ => false,
        };

#endregion


    public override int GetHashCode()
    {
        if (_isLeft)
            return Hasher.GetHashCode<TLeft>(_left);
        return Hasher.GetHashCode<TRight>(_right);
    }

    public override string ToString()
    {
        if (_isLeft)
            return $"Left({_left})";
        return $"Right({_right})";
    }
}