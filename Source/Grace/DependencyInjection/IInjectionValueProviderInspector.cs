using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Implement this interface if you want to provide an overriding value for an injection
    /// </summary>
    public interface IInjectionValueProviderInspector
    {
        /// <summary>
        /// Provide an IExportValueProvider that will be used to resolve the value for 
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="targetInfo"></param>
        /// <param name="valueProvider"></param>
        /// <param name="exportStrategyFilter"></param>
        /// <param name="locateKey"></param>
        /// <returns></returns>
        IExportValueProvider GetValueProvider(IInjectionScope scope,
                                              IInjectionTargetInfo targetInfo,
                                              IExportValueProvider valueProvider,
                                              ExportStrategyFilter exportStrategyFilter,
                                              ILocateKeyValueProvider locateKey);
    }
}
