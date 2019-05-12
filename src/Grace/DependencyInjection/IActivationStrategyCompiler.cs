using System;
using System.Linq.Expressions;
using Grace.DependencyInjection.Impl.Expressions;

namespace Grace.DependencyInjection
{
    /// <summary>
    /// Compiler to create ActivationStrategyDelegate delegates
    /// </summary>
    public interface IActivationStrategyCompiler
    {
        /// <summary>
        /// Max object graph depth
        /// </summary>
        int MaxObjectGraphDepth { get; }

        /// <summary>
        /// Creates a new expression request
        /// </summary>
        /// <param name="activationType">activation type</param>
        /// <param name="objectGraphDepth">current object depth</param>
        /// <param name="requestingScope">requesting scope</param>
        /// <returns>request</returns>
        IActivationExpressionRequest CreateNewRequest(Type activationType, int objectGraphDepth, IInjectionScope requestingScope);

        /// <summary>
        /// Create a new expresion result
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="expression">expression</param>
        /// <returns></returns>
        IActivationExpressionResult CreateNewResult(IActivationExpressionRequest request, Expression expression = null);

        /// <summary>
        /// Find a delegate for a specific type
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="locateType"></param>
        /// <param name="consider"></param>
        /// <param name="key"></param>
        /// <param name="forMissingType"></param>
        /// <param name="checkForMissingType"></param>
        /// <returns></returns>
        ActivationStrategyDelegate FindDelegate(IInjectionScope scope, Type locateType, ActivationStrategyFilter consider, object key, IInjectionContext forMissingType, bool checkForMissingType);
        
        /// <summary>
        /// Compile a delegate
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="expressionContext"></param>
        /// <returns></returns>
        ActivationStrategyDelegate CompileDelegate(IInjectionScope scope, IActivationExpressionResult expressionContext);

        /// <summary>
        /// Compile fast instance creation delegate that can be used when certain parameters aren't needed.
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="expressionContext"></param>
        /// <returns></returns>
        T CompileOptimizedDelegate<T>(IInjectionScope scope, IActivationExpressionResult expressionContext);

        /// <summary>
        /// Create injection delegate 
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="locateType"></param>
        /// <returns></returns>
        InjectionStrategyDelegate CreateInjectionDelegate(IInjectionScope scope, Type locateType);

        /// <summary>
        /// Process missing strategy providers
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        void ProcessMissingStrategyProviders(IInjectionScope scope, IActivationExpressionRequest request);

        /// <summary>
        /// Default strategy expression builder
        /// </summary>
        IDefaultStrategyExpressionBuilder DefaultStrategyExpressionBuilder { get; }
    }
}
