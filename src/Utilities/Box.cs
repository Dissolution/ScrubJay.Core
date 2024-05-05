//using ScrubJay.Comparison;
//
//namespace ScrubJay.Utilities;
//
//public sealed class Box :
//#if NET7_0_OR_GREATER
//    IEqualityOperators<Box, Box, bool>,
//    IEqualityOperators<Box, object, bool>,
//    IComparisonOperators<Box, Box, bool>,
//    IComparisonOperators<Box, object, bool>,
//#endif
//    IEquatable<Box>,
//    IComparable<Box>, IComparable,
//    IFormattable
//{
//    public static bool operator ==(Box? left, Box? right) => Relate.Equal.Values<Box>(left, right);
//    public static bool operator ==(Box? box, object? obj) => box is null ? obj is null : box.Equals(obj); 
//    public static bool operator ==(object? obj, Box? box) => box is null ? obj is null : box.Equals(obj);
//
//    public static bool operator !=(Box? left, Box? right) => !Relate.Equal.Values<Box>(left, right);
//    public static bool operator !=(Box? box, object? obj) => box is null ? obj is not null : !box.Equals(obj); 
//    public static bool operator !=(object? obj, Box? box) => box is null ? obj is not null : !box.Equals(obj);
//
//    public static bool operator >(Box? left, Box? right) => Relate.Compare.Values<Box>(left, right) > 0;
//    public static bool operator >(Box box, object? obj) => box.CompareTo(obj) > 0;
//    public static bool operator >(object? obj, Box box) => box.CompareTo(obj) <= 0;
//
//    public static bool operator <(Box? left, Box? right) => Relate.Compare.Values<Box>(left, right) < 0;
//    public static bool operator <(Box box, object? obj) => box.CompareTo(obj) < 0;
//    public static bool operator <(object? obj, Box box) => box.CompareTo(obj) >= 0;
//
//    public static bool operator >=(Box? left, Box? right) => Relate.Compare.Values<Box>(left, right) >= 0;
//    public static bool operator >=(Box box, object? obj) => box.CompareTo(obj) >= 0;
//    public static bool operator >=(object? obj, Box box) => box.CompareTo(obj) < 0;
//
//    public static bool operator <=(Box? left, Box? right) => Relate.Compare.Values<Box>(left, right) <= 0;
//    public static bool operator <=(Box box, object? obj) => box.CompareTo(obj) <= 0;
//    public static bool operator <=(object? obj, Box box) => box.CompareTo(obj) > 0;
//
//
//    public static Box Create<T>(T value)
//    {
//        var type = typeof(T);
//        if (type == typeof(object) && value is not null)
//        {
//            type = value.GetType();
//        }
//
//        return new Box((object?)value, type);
//    }
//
//
//    private readonly object? _boxedValue;
//    private readonly Type _valueType;
//
//    public bool ContainsNull => _boxedValue is null;
//
//    private Box(object? boxedValue, Type valueType)
//    {
//        _boxedValue = boxedValue;
//        _valueType = valueType;
//    }
//
//    public bool Is<T>()
//        where T : notnull
//    {
//        return _boxedValue is T;
//    }
//
//    public bool Is(Type? type)
//    {
//        if (type is null) return ContainsNull;
//        return _valueType == type;
//    }
//    
//    public bool Is<T>([NotNullWhen(true)] out T? value)
//        where T : notnull
//    {
//        return _boxedValue.Is<T>(out value);
//    }
//
//    public bool As<T>()
//    {
//        return _boxedValue.CanBe<T>();
//    }
//
//    public bool As(Type? type)
//    {
//        if (type is null) return _valueType.CanContainNull();
//        return _valueType.Implements(type);
//    }
//
//    public bool As<T>(out T? value)
//    {
//        return _boxedValue.CanBe<T>(out value);
//    }
//
//  
//
//    public ref T Ref<T>()
//        where T : notnull
//    {
//        if (_boxedValue is T)
//            return ref Scary.UnboxRef<T>(_boxedValue);
//        throw new InvalidOperationException("Cannot get a ref to underlying value");
//    }
//
//    public int CompareTo(Box? box)
//    {
//        return Relate.Compare.GetDefaultComparer(_valueType).Compare(_boxedValue, box?._boxedValue);
//    }
//
//    public int CompareTo(object? obj)
//    {
//        return Relate.Compare.GetDefaultComparer(_valueType).Compare(_boxedValue, obj);
//    }
//
//    public bool Equals(Box? box)
//    {
//        return Relate.Equal.GetEqualityComparer(_valueType).Equals(_boxedValue, box?._boxedValue);
//    }
//
//    public bool Equals<T>(T? value)
//    {
//        return As<T>(out var unboxedValue) && EqualityComparer<T>.Default.Equals(unboxedValue, value);
//    }
//
//    public override bool Equals(object? obj)
//    {
//        if (ReferenceEquals(_boxedValue, obj)) return true;
//        if (obj is null) return false;
//        if (obj is Box box) return Equals(box);
//        var objType = obj.GetType();
//        if (objType != _valueType) return false;
//        return Relate.Equal.GetEqualityComparer(_valueType).Equals(_boxedValue, obj);
//    }
//
//    public override int GetHashCode()
//    {
//        return Hasher.GetHashCode<object?>(_boxedValue);
//    }
//
//    public string ToString(string? format, IFormatProvider? provider = default)
//    {
//        string? valueStr;
//        if (_boxedValue is IFormattable formattable)
//        {
//            valueStr = formattable.ToString(format, provider);
//        }
//        else
//        {
//            valueStr = _boxedValue?.ToString();
//        }
//
//        return $"Box<{_valueType}>({valueStr})";
//    }
//
//    public override string ToString()
//    {
//        return $"Box<{_valueType}>({_boxedValue})";
//    }
//}