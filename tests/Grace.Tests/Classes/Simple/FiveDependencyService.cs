namespace Grace.Tests.Classes.Simple
{
    public interface IFiveDependencyService<T1, T2, T3, T4, T5>
    {
        T1 Dependency1 { get; }
        T2 Dependency2 { get; }
        T3 Dependency3 { get; }
        T4 Dependency4 { get; }
        T5 Dependency5 { get; }
    }

    public class FiveDependencyService<T1, T2, T3, T4, T5> : IFiveDependencyService<T1, T2, T3, T4, T5>
    {
        public FiveDependencyService(T1 dependency1, T2 dependency2, T3 dependency3, T4 dependency4, T5 dependency5)
        {
            Dependency1 = dependency1;
            Dependency2 = dependency2;
            Dependency3 = dependency3;
            Dependency4 = dependency4;
            Dependency5 = dependency5;
        }


        public T1 Dependency1 { get; }

        public T2 Dependency2 { get; }

        public T3 Dependency3 { get; }

        public T4 Dependency4 { get; }

        public T5 Dependency5 { get; }
    }
}
