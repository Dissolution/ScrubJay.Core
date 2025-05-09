﻿using System.Globalization;

namespace ScrubJay.Text.Rendering;

[PublicAPI]
public sealed class GuidRenderer : Renderer<Guid>
{
    public override void RenderTo<B>(Guid guid, B builder)
    {
        builder.Allocate(32, buffer =>
        {
#if NETFRAMEWORK || NETSTANDARD2_0
            string str = guid.ToString("N");
            Notsafe.Text.CopyBlock(str, buffer, 32);
#else
            guid.TryFormat(buffer, out _, format: "N");
#endif
            buffer.ForEach((ref char ch) => ch = char.ToUpper(ch, CultureInfo.InvariantCulture));
        });
    }
}