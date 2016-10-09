using System.Linq.Expressions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.InstanceStrategies
{
    public class ConstantInstanceExportStrategy<T> : BaseInstanceExportStrategy
    {
        private readonly T _constant;

        public ConstantInstanceExportStrategy(T constant, IInjectionScope injectionScope) : base(typeof(T), injectionScope)
        {
            _constant = constant;
        }

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
