using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Grace.DependencyInjection.Impl.Wrappers
{
    public class MetaWrapperStrategy : BaseWrapperStrategy, ICompiledWrapperStrategy
    {
        public MetaWrapperStrategy(IInjectionScope injectionScope) : base(typeof(Meta<>), injectionScope)
        {
        }
        
        public override Type GetWrappedType(Type wrappedType)
        {
            if (wrappedType.IsConstructedGenericType)
            {
                var genericType = wrappedType.GetGenericTypeDefinition();

                if (genericType == typeof(Meta<>))
                {
                    return wrappedType.GetTypeInfo().GenericTypeArguments[0];
                }
            }

            return null;
        }
        public override IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var requestType = request.ActivationType.GetTypeInfo().GenericTypeArguments[0];

            var constructor = request.ActivationType.GetTypeInfo().DeclaredConstructors.First();

            var newRequest = request.NewRequest(requestType, request.InjectedType, RequestType.Other, null, true);

            var strategy = request.GetWrappedExportStrategy();

            if (strategy == null)
            {
                throw new Exception("Could not find export stragegy to get metadata from");
            }

            var expressionResult = request.Services.ExpressionBuilder.GetActivationExpression(scope, newRequest);

            var newExpression =
                Expression.New(constructor, expressionResult.Expression, Expression.Constant(strategy.Metadata));

            var newResult = request.Services.Compiler.CreateNewResult(request, newExpression);

            newResult.AddExpressionResult(expressionResult);

            return newResult;
        }

        protected override ActivationStrategyDelegate CompileDelegate(IInjectionScope scope, IActivationStrategyCompiler compiler,
            Type activationType)
        {
            var request = compiler.CreateNewRequest(activationType, 1);

            var expressionResult = request.Services.ExpressionBuilder.GetActivationExpression(scope, request);

            return compiler.CompileDelegate(scope, expressionResult);
        }
    }
}
