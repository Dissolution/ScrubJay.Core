namespace ScrubJay.Fluent;

[PublicAPI]
public abstract class FluentRecordBuilder<B, R> : FluentBuilder<B>
    where B : FluentRecordBuilder<B, R>
    where R : class
{
    public static implicit operator R(FluentRecordBuilder<B, R> builder) => builder.Record;


    /// <summary>
    /// Gets the <typeparamref name="R"/> being built
    /// </summary>
    public R Record { get; }

    protected FluentRecordBuilder(R record) : base()
    {
        this.Record = record;
    }

    public virtual B Execute(Action<B, R>? buildWithRecord)
    {
        buildWithRecord?.Invoke(_builder, Record);
        return _builder;
    }
}
