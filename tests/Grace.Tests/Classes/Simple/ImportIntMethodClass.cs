namespace Grace.Tests.Classes.Simple
{
    public class ImportIntMethodClass
    {
        public void SetValue(int value)
        {
            Value = value;
        }

        public int Value { get; private set; }
    }
}
