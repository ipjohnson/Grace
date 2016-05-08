using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Allows for the configuration of a constructor param
    /// </summary>
    public interface IExportTypeSetCtorParamConfiguration : IExportTypeSetConfiguration
    {
        /// <summary>
        /// Applies a filter to be used when resolving a parameter constructor
        /// It will be called each time the parameter is resolved
        /// </summary>
        /// <param name="filterFunc">filter delegate to be used when resolving parameter</param>
        /// <returns>configuration object</returns>
        IExportTypeSetCtorParamConfiguration Consider([NotNull]Func<Type, ExportStrategyFilter> filterFunc);

        /// <summary>
        /// Default value if one cannot be located
        /// </summary>
        /// <param name="defaultValueFunc">default value</param>
        /// <returns></returns>
        IExportTypeSetCtorParamConfiguration DefaultValue([NotNull] Func<Type, object> defaultValueFunc);

        /// <summary>
        /// Name to use when resolving parameter
        /// </summary>
        /// <param name="importNameFunc"></param>
        /// <returns>configuration object</returns>
        IExportTypeSetCtorParamConfiguration ImportName([NotNull] Func<Type, string> importNameFunc);

        /// <summary>
        /// Is the parameter required when resolving the type
        /// </summary>
        /// <param name="isRequiredFunc">is the parameter required</param>
        /// <returns>configuration object</returns>
        IExportTypeSetCtorParamConfiguration IsRequired([CanBeNull] Func<Type, bool> isRequiredFunc = null);

        /// <summary>
        /// Locate with a particular key
        /// </summary>
        /// <param name="locateWithKeyFunc">ocate key</param>
        /// <returns>configuration object</returns>
        IExportTypeSetCtorParamConfiguration LocateWithKey([NotNull] Func<Type, object> locateWithKeyFunc);

        /// <summary>
        /// Locate with a particular key
        /// </summary>
        /// <param name="locateWithKeyFunc">ocate key</param>
        /// <returns>configuration object</returns>
        IExportTypeSetCtorParamConfiguration LocateWithKeyProvider([NotNull] Func<IInjectionScope, IInjectionContext, Type, object> locateWithKeyFunc);

        /// <summary>
        /// Locate with a particular key
        /// </summary>
        /// <param name="locateKeyValueProvider">locate key</param>
        /// <returns>configuration object</returns>
        IExportTypeSetCtorParamConfiguration LocateWithKeyProvider([NotNull] ILocateKeyValueProvider locateKeyValueProvider);

        /// <summary>
        /// Name of the parameter to resolve
        /// </summary>
        /// <param name="namedFunc"></param>
        /// <returns>configuration object</returns>
        IExportTypeSetCtorParamConfiguration Named([NotNull] Func<Type, string> namedFunc);

        /// <summary>
        /// Provides a value for a constructor parameter
        /// </summary>
        /// <param name="valueProviderFunc">value provider for parameter</param>
        /// <returns>configuration object</returns>
        IExportTypeSetCtorParamConfiguration UsingValueProvider([NotNull] Func<Type, IExportValueProvider> valueProviderFunc);
    }
}
