namespace Grace.DependencyInjection.Conditions
{
    /// <summary>
    /// used to test if an activation strategy should be used
    /// </summary>
    public interface ICompiledCondition
    {
        /// <summary>
        /// Should the condition be run at expression creation time or every time a request is made for the type
        /// </summary>
        bool IsRequestTimeCondition { get; }

        /// <summary>
        /// If it is a request time condition does it need an injection context
        /// </summary>
        bool RequiresInjectionContext { get; }

        /// <summary>
        /// Test if strategy meets condition
        /// </summary>
        /// <param name="strategy">strategy to test</param>
        /// <param name="staticInjectionContext">static injection context</param>
        /// <param name="context"></param>
        /// <returns>meets condition</returns>
        bool MeetsCondition(IActivationStrategy strategy, StaticInjectionContext staticInjectionContext, IInjectionContext context = null);
    }
}
