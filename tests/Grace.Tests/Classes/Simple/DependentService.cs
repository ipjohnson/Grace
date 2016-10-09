namespace Grace.Tests.Classes.Simple
{
    public interface IDependentService<T>
    {
        T Value { get; }
    }

    public class DependentService<T> : IDependentService<T>
    {
        public DependentService(T value)
        {
            Value = value;
        }

        public T Value { get; }
    }
}
