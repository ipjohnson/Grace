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
    {		/// <summary>
        /// Type being constructed
        /// </summary>
        Type ActivationType { get; }

        /// <summary>
        /// The IInjectionScope this strategy is being call with
        /// </summary>
        ParameterExpression ExportStrategyScopeParameter { get; }

        /// <summary>
        /// Injection context parameter (IInjectionContext)
        /// </summary>
        ParameterExpression InjectionContextParameter { get; }

        /// <summary>
        /// Instance being created (object)
        /// </summary>
        ParameterExpression InstanceVariable { get; }

        /// <summary>
        /// Parameters added usingt his method will be added to the main body of the delegate
        /// </summary>
        /// <param name="newLocalVariable">new local variable</param>
        void AddLocalVariable(ParameterExpression newLocalVariable);

        /// <summary>
        /// Creates the default Constructor Expression
        /// </summary>
        /// <param name="constructorInfo"></param>
        /// <param name="constructorParameters"></param>
        /// <returns></returns>
        Expression GetConstructorExpression(out ConstructorInfo constructorInfo, out IEnumerable<Expression> constructorParameters);

        /// <summary>
        /// Creates a Locate expression to be used in custom construction
        /// </summary>
        /// <param name="importType"></param>
        /// <param name="targetInfo"></param>
        /// <param name="importName"></param>
        /// <param name="variableName"></param>
        /// <param name="isRequired"></param>
        /// <param name="valueProvider"></param>
        /// <param name="exportStrategyFilter"></param>
        /// <param name="locateKey"></param>
        /// <param name="comparerObject"></param>
        /// <returns></returns>
        ParameterExpression GetLocateDependencyExpression(Type importType = null,
                                                          string importName = null,
                                                          IInjectionTargetInfo targetInfo = null,
                                                          string variableName = null,
                                                          bool isRequired = true,
                                                          IExportValueProvider valueProvider = null,
                                                          ExportStrategyFilter exportStrategyFilter = null,
                                                          ILocateKeyValueProvider locateKey = null,
                                                          object comparerObject = null);
    }
}
