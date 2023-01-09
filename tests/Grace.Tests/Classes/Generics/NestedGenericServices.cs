namespace Grace.Tests.Classes.Generics
{
    public interface IGenericServiceA<in T>
    {
        string GetValue(T value);
    }

    public class GenericServiceA<T> : IGenericServiceA<T>
    {
        private readonly GenericServiceB<T> _genericServiceB;

        public GenericServiceA(GenericServiceB<T> genericServiceB)
        {
            _genericServiceB = genericServiceB;
        }

        public string GetValue(T value)
        {
            return _genericServiceB.GetValue(value);
        }
    }

    public interface IGenericServiceB<in T>
    {
        string GetValue(T value);
    }

    public class GenericServiceB<T> : IGenericServiceB<T>
    {
        public string GetValue(T value)
        {
            return value.ToString();
        }
    }
}
