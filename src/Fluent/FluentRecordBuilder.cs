#pragma warning disable CA1715

namespace ScrubJay.Fluent;

[PublicAPI]
public abstract class FluentRecordBuilder<B, R> : FluentBuilder<B>,
    IEquatable<R>
    where B : FluentRecordBuilder<B, R>
    where R : class
{
    public static implicit operator R(FluentRecordBuilder<B, R> builder) => builder._record;

    protected R _record;

    /// <summary>
    /// Gets the <typeparamref name="R"/> being built
    /// </summary>
    public R Record => _record;

    protected FluentRecordBuilder() : base()
    {
        _record = new();
    }

    protected FluentRecordBuilder(R record) : base()
    {
        _record = record;
    }

    public virtual B Execute(Action<B, R> buildWithRecord)
    {
        buildWithRecord.Invoke(_builder, _record);
        return _builder;
    }

    public bool Equals(R? record) => EqualityComparer<R>.Default.Equals(_record!, record!);

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is B)
            return ReferenceEquals(this, obj);
        if (obj is R record)
            return Equals(record);
        return false;
    }

    public override int GetHashCode() => Hasher.GetHashCode<R>(_record);

    public override string ToString() => $"{typeof(B).Name}<{typeof(R).Name}>";
}