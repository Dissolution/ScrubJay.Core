using System.Reflection;
using System.Reflection.Emit;

namespace ScrubJay.Rendering;

partial class Renderer
{
    private static readonly ConcurrentTypeMap<ValueRenderer<object>> _renderObjectMap = [];

    private static ValueRenderer<object> CreateObjectRenderer(Type type)
    {
        var renderToMethod = typeof(Renderer)
            .GetMethod(nameof(RenderTo), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
            .ThrowIfNull("Could not find Renderer.RenderTo")
            .MakeGenericMethod(type);

        var dm = new DynamicMethod(
            name: $"renderBoxed{type}",
            attributes: MethodAttributes.Public | MethodAttributes.Static,
            callingConvention: CallingConventions.Standard,
            returnType: typeof(void),
            parameterTypes: [typeof(object), typeof(TextBuilder)],
            m: typeof(Renderer).Module,
            skipVisibility: true);
        var gen = dm.GetILGenerator();
        gen.Emit(OpCodes.Ldarg_0);
        gen.Emit(OpCodes.Unbox_Any, type);
        gen.Emit(OpCodes.Ldarg_1);
        gen.Emit(OpCodes.Call, renderToMethod);
        gen.Emit(OpCodes.Ret);
        return dm.CreateDelegate<ValueRenderer<object>>();
    }

    private static void RenderObjectTo(object obj, TextBuilder builder)
    {
        Type objType = obj.GetType();
        if (objType == typeof(object))
        {
            // prevent recursion
            builder.Write("〈object〉");
            return;
        }

        var objectRenderer = _renderObjectMap.GetOrAdd(objType, CreateObjectRenderer);
        objectRenderer.Invoke(obj, builder);
    }
}