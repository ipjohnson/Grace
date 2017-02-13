using System;
using Grace.DependencyInjection.Impl.CompiledStrategies;

namespace Grace.DependencyInjection.Extensions
{
    /// <summary>
    /// C# registration block extensions
    /// </summary>
    public static class RegistrationBlockExtensions
    {
        /// <summary>
        /// Directly export a function with no decoratory, no testing for null, no lifestyle. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="block"></param>
        /// <param name="func"></param>
        public static void ExportFunc<T>(this IExportRegistrationBlock block, Func<IExportLocatorScope, T> func)
        {
            block.AddActivationStrategy(new SimpleFuncExportStrategy<T>(func, block.OwningScope));
        }
    }
}
