using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Provides a mechanism for requesting an Expression that will import a specified type
    /// </summary>
    public interface ICustomConstructorEnrichmentLinqExpressionContext
    {
        /// <summary>
        /// Creates the default Constructor Expression
        /// </summary>
        /// <param name="constructorInfo"></param>
        /// <param name="constructorParameters"></param>
        /// <returns></returns>
        BlockExpression GetConstructorExpression(out ConstructorInfo constructorInfo, out IEnumerable<ParameterExpression> constructorParameters);

        /// <summary>
        /// Creates a Locate expression to be used in custom construction
        /// </summary>
        /// <param name="importType"></param>
        /// <param name="targetInfo"></param>
        /// <param name="exportName"></param>
        /// <param name="variableName"></param>
        /// <param name="isRequired"></param>
        /// <param name="valueProvider"></param>
        /// <param name="exportStrategyFilter"></param>
        /// <param name="locateKey"></param>
        /// <param name="comparerObject"></param>
        /// <returns></returns>
        ParameterExpression GetLocateDependencyExpression(Type importType = null,
                                                          IInjectionTargetInfo targetInfo = null,
                                                          string exportName = null,
                                                          string variableName = null,
                                                          bool isRequired = true,
                                                          IExportValueProvider valueProvider = null,
                                                          ExportStrategyFilter exportStrategyFilter = null,
                                                          ILocateKeyValueProvider locateKey = null,
                                                          object comparerObject = null);
    }
}
