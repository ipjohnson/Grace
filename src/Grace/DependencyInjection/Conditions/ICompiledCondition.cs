namespace Grace.DependencyInjection.Conditions
{
    /// <summary>
    /// used to test if an activation strategy should be used
    /// </summary>
    public interface ICompiledCondition
    {
        /// <summary>
        /// Test if strategy meets condition at configuration time
        /// </summary>
        /// <param name="strategy">strategy to test</param>
        /// <param name="staticInjectionContext">static injection context</param>
        /// <returns>meets condition</returns>
        bool MeetsCondition(IActivationStrategy strategy, StaticInjectionContext staticInjectionContext);
    }
}
