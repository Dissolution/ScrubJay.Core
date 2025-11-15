namespace ScrubJay.Rendering;

[PublicAPI]
public static class RenderingExtensions
{
    extension(Enum e)
    {
        public string Display()
        {
            var memberInfo = EnumMemberInfo.For(e);
            if (memberInfo is not null)
            {
                return memberInfo.Display;
            }

            return Enum.GetName(e.GetType(), e) ?? e.ToString();
        }
    }
}