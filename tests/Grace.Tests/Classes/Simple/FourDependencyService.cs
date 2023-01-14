namespace Grace.Tests.Classes.Simple
{
    public interface IFourDependencyService<out T1, out T2, out T3, out T4>
    {
        T1 Dependency1 { get; }
        T2 Dependency2 { get; }
        T3 Dependency3 { get; }
        T4 Dependency4 { get; }
    }

    public class FourDependencyService<T1,T2,T3,T4> : IFourDependencyService<T1, T2, T3, T4>
    {
        public FourDependencyService(T1 dependency1, T2 dependency2, T3 dependency3, T4 dependency4)
        {
            Dependency1 = dependency1;
            Dependency2 = dependency2;
            Dependency3 = dependency3;
            Dependency4 = dependency4;
        }

        public T1 Dependency1 { get; }

        public T2 Dependency2 { get; }

        public T3 Dependency3 { get; }

        public T4 Dependency4 { get; }
    }
}
