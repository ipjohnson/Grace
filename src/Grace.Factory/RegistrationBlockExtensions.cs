using System;
using System.Collections.Generic;
using System.Text;
using Grace.DependencyInjection;
using Grace.Factory.Impl;

namespace Grace.Factory
{
    public static class RegistrationBlockExtensions
    {
        public static IExportRegistrationBlock ExportInterfaceFactories(this IExportRegistrationBlock block, Func<Type, bool> filter = null)
        {
            block.AddMissingExportStrategyProvider(new FactoryStrategyProvider(filter));

            return block;
        }

        public static IExportRegistrationBlock ExportFactory<T>(this IExportRegistrationBlock block)
        {
            block.AddActivationStrategy(new DynamicFactoryStrategy(typeof(T), block.OwningScope));

            return block;
        }
    }
}
