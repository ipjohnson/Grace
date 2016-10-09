namespace Grace.DependencyInjection
{
    public interface IActivationStrategyInspector
    {
        void Inspect<T>(T strategy) where T : class, IActivationStrategy;
    }
}
