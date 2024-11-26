using ScrubJay.Comparison;

#pragma warning disable CA1715

namespace ScrubJay.Fluent;

[PublicAPI]
public abstract class FluentRecordBuilder<B, R> : FluentBuilder<B>
    where B : FluentRecordBuilder<B, R>
    where R : class
{
    public static implicit operator R(FluentRecordBuilder<B, R> builder) => builder._record;

    protected R _record;

    /// <summary>
    /// Gets the <typeparamref name="R"/> being built
    /// </summary>
    public R Record => _record;

    protected FluentRecordBuilder(R record) : base()
    {
        _record = record;
    }

    public virtual B Execute(Action<B, R>? buildWithRecord)
    {
        buildWithRecord?.Invoke(_builder, _record);
        return _builder;
    }

    public override bool Equals([NotNullWhen(true)] object? obj) => obj switch
    {
        B builder => ReferenceEquals(_builder, builder),
        R record => Equate.Values(_record, record),
        _ => false,
    };

    public override string ToString() => $""



    public override int GetHashCode() => base.GetHashCode();
}