using System.Reflection;

namespace ScrubJay.Rendering;

partial class Renderer
{
    private static void RenderObjectTo(object obj, TextBuilder builder)
    {
        Type objType = obj.GetType();
        if (objType == typeof(object))
        {
            // prevent recursion
            builder.Write("〈object〉");
            return;
        }

        var result = typeof(Renderer).GetMethod(nameof(FindValueRenderer),
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
            .ThrowIfNull()
            .MakeGenericMethod(objType)
            .Invoke(null, null)
            .ThrowIfNot<Delegate>()
            .DynamicInvoke(obj, builder);

        Debugger.Break();
    }
}