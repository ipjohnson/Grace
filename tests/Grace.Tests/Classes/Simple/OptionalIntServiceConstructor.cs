namespace Grace.Tests.Classes.Simple
{
    public interface IOptionalServiceConstructor
    {
        ISimpleObject SimpleObject { get; }
    }

    public class OptionalServiceConstructor : IOptionalServiceConstructor
    {
        public OptionalServiceConstructor(ISimpleObject simpleObject = null)
        {
            SimpleObject = simpleObject;
        }

        public ISimpleObject SimpleObject { get; private set; }
    }

    public interface IOptionalIntServiceConstructor
    {
        ISimpleObject SimpleObject { get; }

        int Value { get; }
    }

    public class OptionalIntServiceConstructor : IOptionalIntServiceConstructor
    {
        public const int DefaultIntValue = 5;

        public OptionalIntServiceConstructor(ISimpleObject simpleObject, int value = DefaultIntValue)
        {
            Value = value;
            SimpleObject = simpleObject;
        }

        public ISimpleObject SimpleObject { get; private set; }

        public int Value { get; private set; }
    }

    public interface IOptionalStringConstructor
    {
        string Value { get; }
    }

    public class OptionalStringConstructor : IOptionalStringConstructor
    {
        public OptionalStringConstructor(string someString = "Blah")
        {
            Value = someString;
        }

        public string Value { get; private set; }
    }
}
