using System;
using Grace.DependencyInjection;
using Grace.Factory.Impl;

namespace Grace.Factory
{
    /// <summary>
    /// C# extensions for registration block
    /// </summary>
    public static class RegistrationBlockExtensions
    {
        /// <summary>
        /// Export all missing interfaces as factories, this can be filtered
        /// </summary>
        /// <param name="block"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static IExportRegistrationBlock ExportInterfaceFactories(this IExportRegistrationBlock block, Func<Type, bool> filter = null)
        {
            block.AddMissingExportStrategyProvider(new FactoryStrategyProvider(filter));

            return block;
        }

        /// <summary>
        /// Export Factory Interface
        /// </summary>
        /// <typeparam name="T">factory interface</typeparam>
        /// <param name="block">registration block</param>
        /// <returns></returns>
        public static IExportRegistrationBlock ExportFactory<T>(this IExportRegistrationBlock block)
        {
            block.AddActivationStrategy(new DynamicFactoryStrategy(typeof(T), block.OwningScope));

            return block;
        }
    }
}
