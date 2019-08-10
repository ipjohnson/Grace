using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Xml;
using Grace.DependencyInjection.Impl.CompiledStrategies;
using Grace.DependencyInjection.Impl.Expressions;

namespace Grace.DependencyInjection.Impl.KnownTypeStrategies
{
    public class ScopedActivationStrategy : ConfigurableActivationStrategy, IConfigurableCompiledWrapperStrategy
    {
        public ScopedActivationStrategy(IInjectionScope injectionScope) : base(typeof(Scoped<>), injectionScope)
        {
            ActivationConfiguration.ExternallyOwned = true;
        }

        public override ActivationStrategyType StrategyType { get; } = ActivationStrategyType.WrapperStrategy;

        public ActivationStrategyDelegate GetActivationStrategyDelegate(IInjectionScope scope, IActivationStrategyCompiler compiler,
            Type activationType)
        {
            var expression = GetActivationExpression(scope, compiler.CreateNewRequest(activationType, 0, scope));

            return compiler.CompileDelegate(scope, expression);
        }

        public IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var type = request.ActivationType.GenericTypeArguments[0];

            var newRequest =
                request.NewRequest(type, this, request.ActivationType, RequestType.Other, null, true, true);

            var expression = request.Services.ExpressionBuilder.GetActivationExpression(scope, newRequest);

            var scopeNameRequest =
                request.NewRequest(typeof(string), this, ActivationType, RequestType.Other, null, true, true);

            scopeNameRequest.SetIsRequired(false);

            var scopeNameExpression =
                request.Services.ExpressionBuilder.GetActivationExpression(scope, scopeNameRequest);

            var compiled = request.Services.Compiler.CompileDelegate(scope, expression);

            var closedType = typeof(Scoped<>).MakeGenericType(type);

            var newExpression = Expression.New(closedType.GetTypeInfo().DeclaredConstructors.Single(),
                request.ScopeParameter, request.InjectionContextParameter, Expression.Constant(compiled), scopeNameExpression.Expression);

            return request.Services.Compiler.CreateNewResult(request, newExpression);
        }

        public Type GetWrappedType(Type type)
        {
            if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Scoped<>))
            {
                return type.GenericTypeArguments[0];
            }

            return null;
        }

        public void SetWrappedType(Type type)
        {

        }

        public void SetWrappedGenericArgPosition(int argPosition)
        {

        }
    }
}
