using System.Reflection;
using InlineIL;
using static InlineIL.IL;

namespace ScrubJay.Rendering;

partial class Renderer
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DefaultNonRefRenderToShim<T>(T value, TextBuilder builder)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        Emit.Ldarg(nameof(value));
        Emit.Ldarg(nameof(builder));
        Emit.Call(new MethodRef(typeof(Renderer), nameof(NonRefRenderTo)).MakeGenericMethod(typeof(T)));
        Emit.Ret();
    }

    private static void NonRefRenderTo<T>(T value, TextBuilder builder)
    {
        if (value is IRenderable)
        {
            ((IRenderable)value).RenderTo(builder);
            return;
        }

        if (value is ITuple tuple)
        {
            RenderTupleTo(tuple, builder);
            return;
        }

        if (value is Type type)
        {
            TypeRenderer.Default.RenderTo(type, builder);
            return;
        }

        if (value is MethodBase method)
        {
            MethodRenderer.Default.RenderTo(method, builder);
            return;
        }

        if (value is ParameterInfo parameter)
        {
            ParameterRenderer.Default.RenderTo(parameter, builder);
            return;
        }

        Debugger.Break();
        // we can .ToString anything with Stringify
        builder.Write(value.Stringify());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DefaultRefRenderTo<T>(T value, TextBuilder builder)
#if NET9_0_OR_GREATER
        where T : allows ref struct
#endif
    {
        // we can .ToString anything with Stringify
        builder.Write(value.Stringify());
    }
}