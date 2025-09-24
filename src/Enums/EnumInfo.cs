#pragma warning disable CA1819

using System.Reflection;
using ScrubJay.Text.Rendering;

namespace ScrubJay.Enums;

[PublicAPI]
public abstract class EnumInfo :
#if NET7_0_OR_GREATER
    IEqualityOperators<EnumInfo, EnumInfo, bool>,
#endif
    IEquatable<EnumInfo>
{
    public static bool operator ==(EnumInfo? left, EnumInfo? right)
        => Equate.Values(left, right);

    public static bool operator !=(EnumInfo? left, EnumInfo? right)
        => !Equate.Values(left, right);


    private static readonly ConcurrentTypeMap<EnumInfo> _enumInfoCache = [];

    private static EnumInfo CreateEnumInfo(Type enumType)
    {
        var enumInfo = typeof(EnumInfo<>)
            .MakeGenericType(enumType)
            .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
            .First()
            .Invoke(parameters: null)
            .ThrowIfNot<EnumInfo>();
        return enumInfo;
    }

    public static EnumInfo For(Type enumType)
    {
        Throw.IfNull(enumType);
        if (!enumType.IsEnum)
            throw Ex.Arg(enumType, $"Invalid Enum Type: {enumType:@}");
        return _enumInfoCache.GetOrAdd(enumType, CreateEnumInfo);
    }

    public static EnumInfo For(Enum @enum)
    {
        Throw.IfNull(@enum);
        return For(@enum.GetType());
    }

    public static EnumInfo<E> For<E>(E _ = default)
        where E : struct, Enum
    {
        return _enumInfoCache.GetOrAdd<E>(CreateEnumInfo).ThrowIfNot<EnumInfo<E>>();
    }


    private readonly EnumMemberInfo[] _members;

    public Attribute[] Attributes { get; }
    public Type EnumType { get; }
    public Type UnderlyingType { get; }
    public bool IsFlags { get; }
    public IEnumerable<EnumMemberInfo> Members => _members;

    protected EnumInfo(Type enumType)
    {
        Debug.Assert(enumType is not null);
        Debug.Assert(enumType!.IsValueType);
        Debug.Assert(enumType.IsEnum);

        EnumType = enumType;
        UnderlyingType = Enum.GetUnderlyingType(enumType);
        Attributes = Attribute.GetCustomAttributes(enumType);
        IsFlags = Attributes.Contains<FlagsAttribute>();

        var members = EnumType.GetFields(BindingFlags.Public | BindingFlags.Static);
        _members = new EnumMemberInfo[members.Length];
        for (var i = 0; i < members.Length; i++)
        {
            var memberField = members[i];
            var ctors =  typeof(EnumMemberInfo<>)
                .MakeGenericType(enumType)
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
            var instance = ctors
                .First()
                .Invoke(parameters: [this, memberField])
                .ThrowIfNot<EnumMemberInfo>();
            Debugger.Break();
            _members[i] = instance;
        }
    }

    public EnumMemberInfo? GetMemberInfo(string? name)
    {
        return _members
            .FirstOrDefault(m => m.HasAlias(name));
    }

    public EnumMemberInfo? GetMemberInfo(long value)
    {
        return _members.FirstOrDefault(m => m.I64Value == value);
    }

    public EnumMemberInfo<E>? GetMemberInfo<E>(E mune)
        where E : struct, Enum
    {
        if (typeof(E).DeclaringType != EnumType)
            throw Ex.Arg(mune, "Enum does not belong to this EnumInfo");

        return _members.OfType<EnumMemberInfo<E>>()
            .FirstOrDefault(m => m.Member.IsEqual(mune));
    }

    public EnumMemberInfo? GetMemberInfo(Enum mune)
    {
        Throw.IfNull(mune);
        if (mune.GetType() != EnumType)
            throw Ex.Arg(mune, "Enum does not belong to this EnumInfo");

        long value = ((IConvertible)mune).ToInt64(null);

        return _members
            .FirstOrDefault(m => m.I64Value == value);
    }

    public bool Equals(EnumInfo? enumInfo)
    {
        return enumInfo is not null && enumInfo.EnumType == EnumType;
    }

    public override bool Equals(object? obj)
    {
        if (obj is EnumInfo enumInfo)
            return Equals(enumInfo);
        if (obj is Type enumType)
            return enumType == EnumType;
        return false;
    }

    public override int GetHashCode()
    {
        return Hasher.Hash(EnumType);
    }

    public override string ToString()
    {
        return EnumType.Render();
    }
}