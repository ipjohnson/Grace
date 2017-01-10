namespace Grace.Tests.Classes.Simple
{
    public interface IThreeDependencyService<T1, T2, T3>
    {
        T1 Dependency1 { get; }
        T2 Dependency2 { get; }
        T3 Dependency3 { get; }
    }

    public class ThreeDependencyService<T1,T2,T3> : IThreeDependencyService<T1, T2, T3>
    {
        public ThreeDependencyService(T1 instance1, T2 instance2, T3 instance3)
        {
            Dependency1 = instance1;
            Dependency2 = instance2;
            Dependency3 = instance3;
        }

        public T1 Dependency1 { get; }

        public T2 Dependency2 { get; }

        public T3 Dependency3 { get; }


    }
}
