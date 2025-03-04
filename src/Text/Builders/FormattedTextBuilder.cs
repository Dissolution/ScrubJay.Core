using ScrubJay.Collections;

namespace ScrubJay.Text.Builders;

[PublicAPI]
public delegate TBuilder AppendFormatted<TBuilder, in T>(
    TBuilder textBuilder,
    T? value,
    string? format = null)
    where TBuilder : FluentTextBuilder<TBuilder>;

[PublicAPI]
public sealed class FormattedTextBuilder : FluentFormattedTextBuilder<FormattedTextBuilder>
{
    private static readonly ConcurrentTypeMap<Delegate> _appendValueCache = [];

    internal static void AddFormat<TBuilder, T>(
        AppendFormatted<TBuilder, T> appendFormatted)
        where TBuilder : FluentTextBuilder<TBuilder>
    {
        _appendValueCache.AddOrUpdate<T>(appendFormatted);
    }

    internal static Result<AppendFormatted<TBuilder, T>, Exception> TryGetFormat<TBuilder, T>()
        where TBuilder : FluentTextBuilder<TBuilder>
    {
        if (_appendValueCache.TryGetValue<T>(out var del))
        {
            if (del is AppendFormatted<TBuilder, T> appendFormatted)
            {
                return appendFormatted;
            }
            return new InvalidOperationException("Del is not the right type");
        }
        return new InvalidOperationException("There is no formatter");
    }
}


public abstract class FluentFormattedTextBuilder<TBuilder> : FluentTextBuilder<TBuilder>
    where TBuilder : FluentFormattedTextBuilder<TBuilder>
{
    #pragma warning disable CA1000
    public static void AddFormat<T>(
        AppendFormatted<TBuilder, T> appendFormatted)
        => FormattedTextBuilder.AddFormat<TBuilder, T>(appendFormatted);

    public override TBuilder Append<T>([AllowNull] T value)
    {
        if (FormattedTextBuilder.TryGetFormat<TBuilder, T>().HasOk(out var appendFormatted))
        {
            return appendFormatted(_builder, value, default);
        }
        else
        {
            return base.Append<T>(value);
        }
    }

    public override TBuilder Format<T>(
        [AllowNull] T value,
        string? format,
        IFormatProvider? provider = null)
    {
        if (FormattedTextBuilder.TryGetFormat<TBuilder, T>().HasOk(out var appendFormatted))
        {
            return appendFormatted(_builder, value, format);
        }
        else
        {
            return base.Format<T>(value, format, provider);
        }
    }

    public override TBuilder Format<T>(
        [AllowNull] T value,
        scoped text format,
        IFormatProvider? provider = null)
    {
        if (FormattedTextBuilder.TryGetFormat<TBuilder, T>().HasOk(out var appendFormatted))
        {
            return appendFormatted(_builder, value, format.AsString());
        }
        else
        {
            return base.Format<T>(value, format, provider);
        }
    }
}
