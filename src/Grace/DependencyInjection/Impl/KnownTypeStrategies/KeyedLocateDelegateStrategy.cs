using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Impl.CompiledStrategies;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.KnownTypeStrategies
{
    public class KeyedLocateDelegateStrategy : ConfigurableActivationStrategy, ICompiledExportStrategy
    {
        private ImmutableHashTree<Type, ActivationStrategyDelegate> _delegates = ImmutableHashTree<Type, ActivationStrategyDelegate>.Empty;

        public KeyedLocateDelegateStrategy(IInjectionScope injectionScope) : base(typeof(KeyedLocateDelegate<,>), injectionScope)
        {
        }

        public override ActivationStrategyType StrategyType { get; } = ActivationStrategyType.FrameworkExportStrategy;

        public IActivationExpressionResult GetDecoratorActivationExpression(IInjectionScope scope, IActivationExpressionRequest request,
            ICompiledLifestyle lifestyle)
        {
            throw new NotSupportedException("Decorators not currently supported on KeyedLocateDelegate<,>");
        }

        public ActivationStrategyDelegate GetActivationStrategyDelegate(IInjectionScope scope, IActivationStrategyCompiler compiler,
            Type activationType)
        {
            var objectDelegate = _delegates.GetValueOrDefault(activationType);

            if (objectDelegate != null)
            {
                return objectDelegate;
            }

            var request = compiler.CreateNewRequest(activationType, 1);

            var expression = GetActivationExpression(scope, request);

            objectDelegate = compiler.CompileDelegate(scope, expression);

            ImmutableHashTree.ThreadSafeAdd(ref _delegates, activationType, objectDelegate);

            return objectDelegate;
        }


        public IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var openMethod = typeof(KeyedLocateDelegateStrategy).GetRuntimeMethod("CreateKeyedDelegate",
                new[] { typeof(IExportLocatorScope) });

            var closedMethod = openMethod.MakeGenericMethod(request.ActivationType.GenericTypeArguments);

            var expression = Expression.Call(null, closedMethod, request.Constants.ScopeParameter);

            return request.Services.Compiler.CreateNewResult(request, expression);
        }

        public IEnumerable<ICompiledExportStrategy> SecondaryStrategies()
        {
            return ImmutableLinkedList<ICompiledExportStrategy>.Empty;
        }

        public static KeyedLocateDelegate<TKey, TValue> CreateKeyedDelegate<TKey, TValue>(
            IExportLocatorScope locatorScope)
        {
            return key => locatorScope.Locate<TValue>(withKey: key);
        }
    }
}
