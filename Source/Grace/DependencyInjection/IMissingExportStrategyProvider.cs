using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Classes that implement this interface can be used to provide missing export strategies
    /// </summary>
    public interface IMissingExportStrategyProvider
    {
        /// <summary>
        /// Provide a list of exports that can be used to satisfy the request import.
        /// Note the exports will be tested used from first to last looking for the first matching export
        /// </summary>
        /// <param name="requestContext">requesting context</param>
        /// <param name="consider"></param>
        /// <param name="locateKey"></param>
        /// <param name="exportName"></param>
        /// <param name="exportType"></param>
        /// <returns>list of exports</returns>
        [NotNull]
        IEnumerable<IExportStrategy> ProvideExports(IInjectionContext requestContext, string exportName,  Type exportType, ExportStrategyFilter consider, object locateKey);
    }
}
