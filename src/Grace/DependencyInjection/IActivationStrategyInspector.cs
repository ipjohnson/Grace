namespace Grace.DependencyInjection
{
    /// <summary>
    /// This interface allows the developer inspect all strategies as they are added
    /// </summary>
    public interface IActivationStrategyInspector
    {
        /// <summary>
        /// Inspect the activation strategy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strategy"></param>
        void Inspect<T>(T strategy) where T : class, IActivationStrategy;
    }
}
