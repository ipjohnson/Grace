using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace Grace.DependencyInjection.Lifestyle
{
    public class SingletonPerScopeLifestyle : ICompiledLifestyle
    {
        protected readonly string UniqueId = Guid.NewGuid().ToString();
        protected ActivationStrategyDelegate CompiledDelegate;
        protected readonly bool ThreadSafe;

        public SingletonPerScopeLifestyle(bool threadSafe)
        {
            ThreadSafe = threadSafe;
        }

        public virtual bool RootRequest { get; } = true;

        public virtual ICompiledLifestyle Clone()
        {
            return new SingletonPerScopeLifestyle(ThreadSafe);
        }

        public virtual IActivationExpressionResult ProvideLifestlyExpression(IInjectionScope scope, IActivationExpressionRequest requst,
            IActivationExpressionResult activationExpression)
        {
            if (CompiledDelegate == null)
            {
                var localDelegate = requst.Services.Compiler.CompileDelegate(scope, activationExpression);

                Interlocked.CompareExchange(ref CompiledDelegate, localDelegate, null);
            }

            MethodInfo getValueFromScopeMethod = typeof(SingletonPerScopeLifestyle).GetRuntimeMethod(ThreadSafe ? "GetValueFromScopeThreadSafe" : "GetValueFromScope", new[]
            {
                typeof(IExportLocatorScope),
                typeof(ActivationStrategyDelegate),
                typeof(string)
            });

            var closedMethod = getValueFromScopeMethod.MakeGenericMethod(requst.ActivationType);

            var expression = Expression.Call(closedMethod,
                                             requst.Constants.ScopeParameter,
                                             Expression.Constant(CompiledDelegate),
                                             Expression.Constant(UniqueId));

            return requst.Services.Compiler.CreateNewResult(requst, expression);
        }

        public static T GetValueFromScope<T>(IExportLocatorScope scope, ActivationStrategyDelegate creationDelegate,
            string uniqueId)
        {
            var value = scope.GetExtraData(uniqueId);

            if (value != null)
            {
                return (T)value;
            }

            value = creationDelegate(scope, scope, null);

            scope.SetExtraData(uniqueId, value);

            return (T)value;
        }

        public static T GetValueFromScopeThreadSafe<T>(IExportLocatorScope scope, ActivationStrategyDelegate creationDelegate, string uniqueId)
        {
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
