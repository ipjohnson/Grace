namespace Grace.Tests.Classes.Simple
{
    public interface ITwoDependencyService<T1,T2>
    {
        T1 Dependency1 { get; }

        T2 Dependency2 { get; }
    }

    public class TwoDependencyService<T1,T2> : ITwoDependencyService<T1,T2>
    {
        public TwoDependencyService(T1 dependency1, T2 dependency2)
        {
            Dependency1 = dependency1;
            Dependency2 = dependency2;
        }

        public T1 Dependency1 { get; }

        public T2 Dependency2 { get; }
    }
}
