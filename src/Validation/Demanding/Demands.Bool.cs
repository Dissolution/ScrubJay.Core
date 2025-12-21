namespace ScrubJay.Validation.Demanding;

partial class Demands
{

    extension(ValidatingValue<bool> validatingBoolean)
    {
        public void IsTrue()
        {
            if (!validatingBoolean.Value)
            {
                throw DemandException.New(validatingBoolean, "was not true");
            }
        }

        public void IsFalse()
        {
            if (validatingBoolean.Value)
            {
                throw DemandException.New(validatingBoolean, "was not false");
            }
        }
    }
}