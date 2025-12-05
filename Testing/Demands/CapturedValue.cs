using System.Runtime.CompilerServices;
using System.Text;
using ScrubJay.Text;

namespace ScrubJay.Functional.Tests;

public readonly ref struct CapturedValue<T>
#if NET9_0_OR_GREATER
    where T : allows ref struct
#endif
{
    public readonly T Value;
    public readonly string ValueName;

    public CapturedValue(T value,
        [CallerArgumentExpression(nameof(value))]
        string? valueName = null)
    {
        Value = value;
        ValueName = valueName ?? "value";
    }

    public StringBuilder AppendTo(StringBuilder builder)
    {
        // Argument "id" (int) `0`
        return builder.Append($"Argument \"{ValueName}\" ({typeof(T)}) `{Value.Stringify()}`");
    }

    public (Type ValueType, string ValueName) ToTuple()
    {
        return (typeof(T), ValueName);
    }
}

public static class DemandExtensions
{
    extension(in CapturedValue<bool> capturedBool)
    {
        public void IsTrue()
        {
            if (capturedBool.Value == true)
                return;
            throw new DemandException<bool>(capturedBool, "must be true");
        }

        public void IsFalse()
        {
            if (capturedBool.Value == false)
                return;
            throw new DemandException<bool>(capturedBool, "must be false");
        }
    }

    extension(in CapturedValue<Type> capturedType)
    {
        public void InheritsFrom(Type type)
        {
            if (capturedType.Value == type)
                return;

            if (type.IsInterface)
            {
                if (capturedType.Value.GetInterfaces().Any(i => i == type))
                    return;
                throw new DemandException<Type>(capturedType, $"must inherit from interface {type}");
            }

            if (type.IsClass)
            {
                Type? baseType = type.BaseType;
                while (baseType is not null)
                {
                    if (baseType == type)
                        return;
                    baseType = baseType.BaseType;
                }

                throw new DemandException<Type>(capturedType, $"must inherit from class {type}");
            }

            throw new DemandException<Type>(capturedType, $"must inherit from uninheritable type {type}");
        }

        public void InheritsFrom<T>() => capturedType.InheritsFrom(typeof(T));

        public void IsEqualTo<T>()
        {
            if (capturedType.Value == typeof(T))
                return;
            throw new DemandException<Type>(capturedType, $"must be equal to {typeof(T)}");
        }
    }


    extension<T>(in CapturedValue<T> captured)
    {
        public void IsEqualTo(in T expected)
        {
            if (EqualityComparer<T>.Default.Equals(captured.Value!, expected!))
                return;
            throw new DemandException<T>(captured, $"must be equal to {expected}");
        }

        public void IsNotEqualTo(in T expected)
        {
            if (!EqualityComparer<T>.Default.Equals(captured.Value!, expected!))
                return;
            throw new DemandException<T>(captured, $"must not be equal to {expected}");
        }
    }

    extension<T>(in CapturedValue<T> captured)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        public void IsNotNull()
        {
            if (captured.Value is not null)
                return;
            throw new DemandException<T>(captured, "must not be null");
        }

        public void IsNull()
        {
            if (captured.Value is null)
                return;
            throw new DemandException<T>(captured, "must be null");
        }
    }
}