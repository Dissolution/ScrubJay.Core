using System.Linq.Expressions;

#pragma warning disable CA1008

namespace ScrubJay.Validation.Demanding;

[Flags]
public enum Comparison
{
    NotEqual = 0,
    LessThan = 1 << 0,
    Equal = 1 << 1,
    GreaterThan = 1 << 2,
}

partial class Demands
{
    extension(Comparison)
    {
        public static Comparison For<T>(T? left, T? right)
        {
            Comparison comparison = default;

            bool equals = EqualityComparer<T>.Default.Equals(left!, right!);
            if (equals)
            {
                comparison.AddFlag(Comparison.Equal);
            }

            int c = Comparer<T>.Default.Compare(left!, right!);
            if (c > 0)
            {
                comparison.AddFlag(Comparison.GreaterThan);
            }
            else if (c < 0)
            {
                comparison.AddFlag(Comparison.LessThan);
            }
            else
            {
                comparison.AddFlag(Comparison.Equal);
            }

            return comparison;
        }
    }

#if NET9_0_OR_GREATER
    extension<T>(ValidatingValue<T> captured)
        where T : allows ref struct
    {
        public void Is(Func<T, bool> predicate, GenericTypeConstraint.AllowsRefStruct<T> _ = default)
        {
            if (!predicate(captured.Value))
            {
                throw DemandException.New(captured, $"did not meet predicate `{predicate:@}`");
            }
        }
    }
#endif


    /// <summary>
    /// Extensions that validate a <see cref="ValidatingValue{T}"/>
    /// </summary>
    /// <param name="captured"></param>
    /// <typeparam name="T"></typeparam>
    extension<T>(ValidatingValue<T> captured)
    {
        public void Is(Expression<Func<T, bool>> predicateExpression)
        {
            var predicate = predicateExpression.Compile();
            if (!predicate(captured.Value))
            {
                throw DemandException.New(captured, $"did not meet predicate `{predicateExpression:@}`");
            }
        }

        public void IsEqualTo(T? other)
        {
            if (!EqualityComparer<T>.Default.Equals(captured.Value!, other!))
            {
                throw DemandException.New(captured, $"was not equal to `{other:@}`");
            }
        }

        public void IsNotEqualTo(T? other)
        {
            if (EqualityComparer<T>.Default.Equals(captured.Value!, other!))
            {
                throw DemandException.New(captured, $"was equal to `{other:@}`");
            }
        }

        public void ComparesTo(T? other, Comparison comparison)
        {
            var c = Comparison.For(captured.Value, other);
            if (c != comparison)
            {
                throw DemandException.New(captured, $"had compared as {c} to `{other:@}` and not {comparison}");
            }
        }

        public void IsGreaterThan(T? other)
        {
            int c = Comparer<T>.Default.Compare(captured.Value!, other!);
            if (c <= 0)
            {
                throw DemandException.New(captured, $"was not greater than `{other:@}`");
            }
        }

        public void IsGreaterThanOrEqualTo(T? other)
        {
            int c = Comparer<T>.Default.Compare(captured.Value!, other!);
            if (c < 0)
            {
                throw DemandException.New(captured, $"was not greater than or equal to `{other:@}`");
            }
        }
    }
}