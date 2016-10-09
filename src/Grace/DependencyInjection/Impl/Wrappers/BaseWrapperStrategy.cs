using System;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Impl.CompiledStrategies;

namespace Grace.DependencyInjection.Impl.Wrappers
{
    public abstract class BaseWrapperStrategy : ConfigurableActivationStrategy, ICompiledWrapperStrategy
    {
        protected ImmutableHashTree<Type, ActivationStrategyDelegate> ActivationDelegates = ImmutableHashTree<Type, ActivationStrategyDelegate>.Empty;

        protected BaseWrapperStrategy(Type activationType, IInjectionScope injectionScope) : base(activationType, injectionScope)
        {

        }

        public abstract Type GetWrappedType(Type type);
        

        public ActivationStrategyDelegate GetActivationStrategyDelegate(IInjectionScope scope,
                                                                        IActivationStrategyCompiler compiler,
                                                                        Type activationType)
        {
            var returnValue = ActivationDelegates.GetValueOrDefault(activationType);

            if (returnValue != null)
            {
                return returnValue;
            }

            returnValue = CompileDelegate(scope, compiler, activationType);

            if (returnValue != null)
            {
                returnValue = ImmutableHashTree.ThreadSafeAdd(ref ActivationDelegates, activationType, returnValue);
            }

            return returnValue;
        }

        public abstract IActivationExpressionResult GetActivationExpression(IInjectionScope scope, IActivationExpressionRequest request);

        protected virtual ActivationStrategyDelegate CompileDelegate(IInjectionScope scope, IActivationStrategyCompiler compiler,
            Type activationType)
        {
            var request = compiler.CreateNewRequest(activationType, 1);

            var expressionResult = GetActivationExpression(scope, request);

            ActivationStrategyDelegate returnValue = null;

            if (expressionResult != null)
            {
                returnValue = compiler.CompileDelegate(scope, expressionResult);
            }

            return returnValue;
        }
    }
}
