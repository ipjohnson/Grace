using System.Linq.Expressions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.InstanceStrategies
{
    /// <summary>
    /// Strategy that represents a constant value
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConstantInstanceExportStrategy<T> : BaseInstanceExportStrategy
    {
        private readonly T _constant;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="constant"></param>
        /// <param name="injectionScope"></param>
        public ConstantInstanceExportStrategy(T constant, IInjectionScope injectionScope) : base(typeof(T), injectionScope)
        {
            _constant = constant;
        }

        /// <summary>
        /// Create expression that is implemented in child class
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="lifestyle"></param>
        /// <returns></returns>
        protected override IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request,
            ICompiledLifestyle lifestyle)
        {
            var expressionStatement = Expression.Constant(_constant);

            var expressionResult = request.Services.Compiler.CreateNewResult(request, expressionStatement);

            if (lifestyle != null)
            {
                expressionResult = lifestyle.ProvideLifestlyExpression(scope, request, expressionResult);
            }

            return expressionResult;
        }
    }
}
