namespace Grace.Tests.Classes.Simple
{
    public interface IDependsOnOneArgDelegate<T1, T2>
    {
        ITwoDependencyService<T1, T2> CreateWithT2(T2 value);
    }

    public class DependsOnOneArgDelegate<T1, T2> : IDependsOnOneArgDelegate<T1, T2>
    {
        private readonly CustomDelegate _func;

        public delegate ITwoDependencyService<T1, T2> CustomDelegate(T2 value);

        public DependsOnOneArgDelegate(CustomDelegate func)
        {
            _func = func;
        }

        public ITwoDependencyService<T1, T2> CreateWithT2(T2 value)
        {
            return _func(value);
        }
    }
    public class DependsOnOneArgDelegate
    {
    }
}
