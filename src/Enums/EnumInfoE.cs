namespace ScrubJay.Enums;

[PublicAPI]
public sealed class EnumInfo<E> : EnumInfo
    where E : struct, Enum
{
    private EnumInfo() : base(typeof(E)) { }

    public new IEnumerable<EnumMemberInfo<E>> Members => base.Members.OfType<EnumMemberInfo<E>>();

    public EnumMemberInfo<E>? GetMemberInfo(E mune)
    {
        return Members
            .FirstOrDefault(m => m.Member.IsEqual(mune));
    }
}