using Grace.DependencyInjection.Impl;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// C# extension class for configurable activation strategy
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public static class IConfigurableActivationStrategyExtensions
    {
        /// <summary>
        /// Process attributes on type and configure strategy based on attributes
        /// </summary>
        /// <param name="strategy">strategy</param>
        /// <returns></returns>
        public static IConfigurableActivationStrategy ProcessAttributeForStrategy(this IConfigurableActivationStrategy strategy)
        {
            strategy.InjectionScope.ScopeConfiguration.Implementation.Locate<IActivationStrategyAttributeProcessor>().ProcessAttributeForConfigurableActivationStrategy(strategy);

            return strategy;
        }
    }
}
