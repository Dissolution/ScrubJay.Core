namespace ScrubJay.Validation.Demanding;

partial class Demands
{
#if NET9_0_OR_GREATER
    extension<T>(ValidatingValue<Span<T>> validatingSpan)
    {
        public void IsEmpty()
        {
            if (!validatingSpan.Value.IsEmpty)
            {
                throw DemandException.New(validatingSpan, "was not empty");
            }
        }
    }

    extension<T>(ValidatingValue<ReadOnlySpan<T>> validatingSpan)
    {
        public void IsEmpty()
        {
            if (!validatingSpan.Value.IsEmpty)
            {
                throw DemandException.New(validatingSpan, "was not empty");
            }
        }
    }
#endif
}