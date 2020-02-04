using System;
using System.Diagnostics;
using System.Linq;
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
        /// Unique int id value
        /// </summary>
        protected readonly int UniqueIntIdValue = UniqueIntId.GetId();

        /// <summary>
        /// Compiled delegate
        /// </summary>
        protected Delegate CompiledDelegate;

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

            request.RequireExportScope();

            if (CompiledDelegate == null)
            {
                // new request as we don't want to carry any info over from parent request
                var newRequest = request.NewRootedRequest(request.ActivationType, scope, true);

                var newResult = activationExpression(newRequest);

                Delegate localDelegate = null;

                if (ThreadSafe || newRequest.ExportScopeRequired() || newRequest.DisposalScopeRequired() ||
                    newRequest.InjectionContextRequired())
                {
                    localDelegate = request.Services.Compiler.CompileDelegate(scope, newResult);
                }
                else
                {
                    var openMethod = GetType().GetTypeInfo().GetDeclaredMethod(nameof(CompileFuncDelage));

                    var closed = openMethod.MakeGenericMethod(newResult.Expression.Type);

                    localDelegate = (Delegate)closed.Invoke(null, new object[] {request, scope, newResult});
                }

                Interlocked.CompareExchange(ref CompiledDelegate, localDelegate, null);
            }

            Expression createExpression;

            if (ThreadSafe)
            {
                var getValueFromScopeMethod = typeof(SingletonPerScopeLifestyle).GetRuntimeMethod(nameof(GetValueFromScopeThreadSafe), new[]
                    {
                        typeof(IExportLocatorScope),
                        typeof(ActivationStrategyDelegate),
                        typeof(string),
                        typeof(bool),
                        typeof(IInjectionContext)
                    });

                var closedMethod = getValueFromScopeMethod.MakeGenericMethod(request.ActivationType);

                createExpression = Expression.Call(closedMethod,
                    request.ScopeParameter,
                    Expression.Constant(CompiledDelegate),
                    Expression.Constant(UniqueId),
                    Expression.Constant(scope.ScopeConfiguration.SingletonPerScopeShareContext),
                    request.InjectionContextParameter);
            }
            else
            {
                var invokeMethodFastLane = CompiledDelegate.GetMethodInfo().GetParameters().Length < 3;

                if (invokeMethodFastLane)
                {
                    var getOrCreateMethod = typeof(IExportLocatorScope).GetTypeInfo()
                        .GetDeclaredMethods(nameof(IExportLocatorScope.GetOrCreateScopedService)).First(m => m.GetParameters().Length == 2);

                    var closedMethod = getOrCreateMethod.MakeGenericMethod(request.ActivationType);

                    createExpression = Expression.Call(request.ScopeParameter, closedMethod,
                        Expression.Constant(UniqueIntIdValue), Expression.Constant(CompiledDelegate));
                }
                else
                {
                    var getOrCreateMethod = typeof(IExportLocatorScope).GetRuntimeMethod(nameof(IExportLocatorScope.GetOrCreateScopedService),
                        new[] {typeof(int), typeof(ActivationStrategyDelegate), typeof(IInjectionContext)});

                    var closedMethod = getOrCreateMethod.MakeGenericMethod(request.ActivationType);

                    createExpression = Expression.Call(request.ScopeParameter, closedMethod,
                        Expression.Constant(UniqueIntIdValue), Expression.Constant(CompiledDelegate),
                        request.InjectionContextParameter);
                }
            }

            local = Expression.Variable(request.ActivationType);

            request.PerDelegateData.SetExtraData("local" + UniqueId, local);

            var assignExpression = Expression.Assign(local, createExpression);

            var result = request.Services.Compiler.CreateNewResult(request, local);

            result.AddExtraParameter(local);
            result.AddExtraExpression(assignExpression);

            return result;
        }

        /// <summary>
        /// Get value from scope using lock
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scope"></param>
        /// <param name="creationDelegate"></param>
        /// <param name="uniqueId"></param>
        /// <param name="shareContext"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static T GetValueFromScopeThreadSafe<T>(IExportLocatorScope scope, ActivationStrategyDelegate creationDelegate, string uniqueId, bool shareContext, IInjectionContext context)
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
                    value = creationDelegate(scope, scope, shareContext ? context : null);

                    scope.SetExtraData(uniqueId, value);
                }
            }

            return (T)value;
        }

        private static Delegate CompileFuncDelage<T>(IActivationExpressionRequest request, IInjectionScope scope,
            IActivationExpressionResult result)
        {
            return request.Services.Compiler.CompileOptimizedDelegate<Func<T>>(scope, result);
        }
    }
}
