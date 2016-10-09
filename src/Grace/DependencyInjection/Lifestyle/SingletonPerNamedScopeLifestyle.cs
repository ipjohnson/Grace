using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Grace.DependencyInjection.Exceptions;

namespace Grace.DependencyInjection.Lifestyle
{
    public class SingletonPerNamedScopeLifestyle : ICompiledLifestyle
    {
        protected readonly string UniqueId = Guid.NewGuid().ToString();
        private readonly string _scopeName;
        protected ActivationStrategyDelegate CompiledDelegate;

        public SingletonPerNamedScopeLifestyle(string scopeName)
        {
            _scopeName = scopeName;
        }

        public bool RootRequest { get; } = true;

        public ICompiledLifestyle Clone()
        {
            return new SingletonPerNamedScopeLifestyle(_scopeName);
        }

        public IActivationExpressionResult ProvideLifestlyExpression(IInjectionScope scope, IActivationExpressionRequest requst,
            IActivationExpressionResult activationExpression)
        {
            if (CompiledDelegate == null)
            {
                var localDelegate = requst.Services.Compiler.CompileDelegate(scope, activationExpression);

                Interlocked.CompareExchange(ref CompiledDelegate, localDelegate, null);
            }

            var getValueFromScopeMethod =
                typeof(SingletonPerNamedScopeLifestyle).GetRuntimeMethod("GetValueFromScope",
                    new[]
                    {
                        typeof(IExportLocatorScope),
                        typeof(ActivationStrategyDelegate),
                        typeof(string),
                        typeof(string),
                        typeof(StaticInjectionContext)
                    });

            var closedMethod = getValueFromScopeMethod.MakeGenericMethod(requst.ActivationType);

            var expression = Expression.Call(closedMethod,
                                             requst.Constants.ScopeParameter,
                                             Expression.Constant(CompiledDelegate),
                                             Expression.Constant(UniqueId),
                                             Expression.Constant(_scopeName),
                                             Expression.Constant(requst.GetStaticInjectionContext()));

            return requst.Services.Compiler.CreateNewResult(requst, expression);
        }

        public static T GetValueFromScope<T>(IExportLocatorScope scope, ActivationStrategyDelegate creationDelegate,
            string uniqueId,
            string scopeName,
            StaticInjectionContext injectionContext)
        {
            while (scope != null)
            {
                if (scope.Name == scopeName)
                {
                    break;
                }

                scope = scope.Parent;
            }

            if (scope == null)
            {
                throw new NamedScopeLocateException(scopeName, injectionContext);
            }

            var value = scope.GetExtraData(uniqueId);

            if (value != null)
            {
                return (T)value;
            }

            lock (scope.GetLockObject(uniqueId))
            {
                value = scope.GetExtraData(uniqueId);

                if (value == null)
                {
                    value = creationDelegate(scope, scope, null);

                    scope.SetExtraData(uniqueId, value);
                }
            }

            return (T)value;
        }
    }
}
