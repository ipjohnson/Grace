namespace Grace.Tests.Classes.Simple
{
    public interface IDependentService<out T>
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

    public class OtherDependentService<T> : IDependentService<T>
    {
        public OtherDependentService(T value)
        {
            Value = value;
        }

        public T Value { get; }
    }

    public class DecoratorDependentService<T> : IDependentService<T>
    {
        private readonly IDependentService<T> _dependentService;

        public DecoratorDependentService( IDependentService<T> dependentService)
        {
            _dependentService = dependentService;
        }

        public T Value => _dependentService.Value;
    }
}
