using Optional;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Impl.CompiledStrategies;
using Grace.DependencyInjection.Lifestyle;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Grace.Tests.ThirdParty.Optional
{
    public class OptionalStrategy : SimpleGenericStrategy
    {
        private readonly MethodInfo _optionNone;
        private readonly MethodInfo _optionSome;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="injectionScope"></param>
        public OptionalStrategy(IInjectionScope injectionScope) : base(typeof(Option<>), injectionScope)
        {
            _optionNone = typeof(Option).GetTypeInfo().DeclaredMethods.First(m => m.Name == "None" && m.GetParameters().Length == 0);
            _optionSome = typeof(Option).GetTypeInfo().DeclaredMethods.First(m => m.Name == "Some" && m.GetGenericArguments().Length == 1);
        }

        /// <summary>
        /// Type of activation strategy
        /// </summary>
        public override ActivationStrategyType StrategyType => ActivationStrategyType.ExportStrategy;

        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="lifestyle"></param>
        public override IActivationExpressionResult GetDecoratorActivationExpression(IInjectionScope scope, IActivationExpressionRequest request, ICompiledLifestyle lifestyle)
        {
            // not sure why you would want to decorate the Option<> it self as you can decorate the T
            throw new NotSupportedException();
        }

        /// <summary>
        /// Get an activation expression for this strategy
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        public override IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var optionalType = request.ActivationType.GenericTypeArguments[0];

            var newRequest = request.NewRequest(optionalType, this, request.ActivationType, RequestType.Other, null, true, true);

            newRequest.SetLocateKey(request.LocateKey);
            newRequest.SetFilter(request.Filter);
            newRequest.SetEnumerableComparer(request.EnumerableComparer);

            var result = request.Services.ExpressionBuilder.GetActivationExpression(scope, newRequest);

            Expression expression;

            if (result.UsingFallbackExpression)
            {
                var closedMethod = _optionNone.MakeGenericMethod(optionalType);

                expression = Expression.Call(closedMethod);
            }
            else
            {
                var closedMethod = _optionSome.MakeGenericMethod(optionalType);

                expression = Expression.Call(closedMethod, result.Expression);
            }

            var expressionResult = request.Services.Compiler.CreateNewResult(request, expression);

            expressionResult.AddExpressionResult(result);

            return expressionResult;
        }
    }
}
