using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Grace.Data;
using Grace.Utilities;

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
        protected readonly string UniqueId = UniqueStringId.Generate();

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
        /// <param name="threadSafe">Under normal circumstances set to false, only set to true if you know there are multiple threads
        ///  accessing scope and you need to make sure only one instance is created</param>
        public SingletonPerScopeLifestyle(bool threadSafe)
        {
            ThreadSafe = threadSafe;
        }

        /// <summary>
        /// Generalization for lifestyle
        /// </summary>
        public LifestyleType LifestyleType { get; } = LifestyleType.Scoped;

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
        public virtual IActivationExpressionResult ProvideLifestyleExpression(IInjectionScope scope, IActivationExpressionRequest request, Func<IActivationExpressionRequest, IActivationExpressionResult> activationExpression)
        {
            var local = request.PerDelegateData.GetExtraDataOrDefaultValue<ParameterExpression>("local" + UniqueId);

            if (local != null)
            {
                return request.Services.Compiler.CreateNewResult(request, local);
            }

            if (CompiledDelegate == null)
            {
                // new request as we don't want to carry any info over from parent request
                var newRequest = request.NewRootedRequest(request.ActivationType, scope, true);

                var localDelegate = request.Services.Compiler.CompileDelegate(scope, activationExpression(newRequest));

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
                                             request.ScopeParameter,
                                             Expression.Constant(CompiledDelegate),
                                             Expression.Constant(UniqueId));

            local = Expression.Variable(request.ActivationType);

            request.PerDelegateData.SetExtraData("local" + UniqueId, local);

            var assignExpression = Expression.Assign(local, expression);

            var result = request.Services.Compiler.CreateNewResult(request, local);

            result.AddExtraParameter(local);
            result.AddExtraExpression(assignExpression);

            return result;
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
            return (T) (scope.GetExtraData(uniqueId) ?? 
                        scope.SetExtraData(uniqueId, creationDelegate(scope, scope, null), false));
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
