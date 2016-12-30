using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace Grace.DependencyInjection.Lifestyle
{
    /// <summary>
    /// Singleton per scope
    /// </summary>
    [DebuggerDisplay("Singleton Per Scope")]
    public class SingletonPerScopeLifestyle : ICompiledLifestyle
    {
        /// <summary>
        /// Unique id
        /// </summary>
        protected readonly string UniqueId = Guid.NewGuid().ToString();

        /// <summary>
        /// Compiled delegate
        /// </summary>
        protected ActivationStrategyDelegate CompiledDelegate;

        /// <summary>
        /// Thread safe
        /// </summary>
        protected readonly bool ThreadSafe;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="threadSafe"></param>
        public SingletonPerScopeLifestyle(bool threadSafe)
        {
            ThreadSafe = threadSafe;
        }

        /// <summary>
        /// Root the request context when creating expression
        /// </summary>
        public virtual bool RootRequest { get; } = true;

        /// <summary>
        /// Clone the lifestyle
        /// </summary>
        /// <returns></returns>
        public virtual ICompiledLifestyle Clone()
        {
            return new SingletonPerScopeLifestyle(ThreadSafe);
        }

        /// <summary>
        /// Provide an expression that uses the lifestyle
        /// </summary>
        /// <param name="scope">scope for the strategy</param>
        /// <param name="request">activation request</param>
        /// <param name="activationExpression">expression to create strategy type</param>
        /// <returns></returns>
        public virtual IActivationExpressionResult ProvideLifestlyExpression(IInjectionScope scope, IActivationExpressionRequest request, Func<IActivationExpressionRequest, IActivationExpressionResult> activationExpression)
        {
            if (CompiledDelegate == null)
            {
                var localDelegate = request.Services.Compiler.CompileDelegate(scope, activationExpression(request));

                Interlocked.CompareExchange(ref CompiledDelegate, localDelegate, null);
            }

            var getValueFromScopeMethod = typeof(SingletonPerScopeLifestyle).GetRuntimeMethod(ThreadSafe ? "GetValueFromScopeThreadSafe" : "GetValueFromScope", new[]
            {
                typeof(IExportLocatorScope),
                typeof(ActivationStrategyDelegate),
                typeof(string)
            });

            var closedMethod = getValueFromScopeMethod.MakeGenericMethod(request.ActivationType);

            var expression = Expression.Call(closedMethod,
                                             request.Constants.ScopeParameter,
                                             Expression.Constant(CompiledDelegate),
                                             Expression.Constant(UniqueId));

            return request.Services.Compiler.CreateNewResult(request, expression);
        }

        /// <summary>
        /// Get value from scope with no lock
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scope"></param>
        /// <param name="creationDelegate"></param>
        /// <param name="uniqueId"></param>
        /// <returns></returns>
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
        
        /// <summary>
        /// Get value from scope using lock
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scope"></param>
        /// <param name="creationDelegate"></param>
        /// <param name="uniqueId"></param>
        /// <returns></returns>
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
