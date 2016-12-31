using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Web;
using Grace.DependencyInjection;
using Grace.DependencyInjection.Lifestyle;
using Grace.MVC.Extensions;

namespace Grace.MVC.DependencyInjection
{
    /// <summary>
    /// Singleton per web request
    /// </summary>
    public class WebSharedPerRequestLifestyle : ICompiledLifestyle
    {
        /// <summary>
        /// Unique id for instance
        /// </summary>
        protected readonly string UniqueId = Guid.NewGuid().ToString();

        /// <summary>
        /// Compiled delegate
        /// </summary>
        protected ActivationStrategyDelegate CompiledDelegate;

        /// <summary>
        /// Clone the lifestyle
        /// </summary>
        /// <returns></returns>
        public ICompiledLifestyle Clone()
        {
            return new WebSharedPerRequestLifestyle();
        }

        /// <summary>
        /// Provide an expression that uses the lifestyle
        /// </summary>
        /// <param name="scope">scope for the strategy</param>
        /// <param name="request">activation request</param>
        /// <param name="activationExpression">expression to create strategy type</param>
        /// <returns></returns>
        public virtual IActivationExpressionResult ProvideLifestlyExpression(IInjectionScope scope, IActivationExpressionRequest request,
            Func<IActivationExpressionRequest, IActivationExpressionResult> activationExpression)
        {
            if (CompiledDelegate == null)
            {
                var localDelegate = request.Services.Compiler.CompileDelegate(scope, activationExpression(request));

                Interlocked.CompareExchange(ref CompiledDelegate, localDelegate, null);
            }

            var getValueFromScopeMethod = typeof(WebSharedPerRequestLifestyle).GetRuntimeMethod("GetValueFromContext", new[]
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
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scope"></param>
        /// <param name="creationDelegate"></param>
        /// <param name="uniqueId"></param>
        /// <returns></returns>
        public static T GetValueFromContext<T>(IExportLocatorScope scope,
            ActivationStrategyDelegate creationDelegate, string uniqueId)
        {
            if (HttpContext.Current != null)
            {
                var value = HttpContext.Current.Items[uniqueId];

                if (value != null)
                {
                    return (T)value;
                }

                value = creationDelegate(scope, MVCDisposalScopeProvider.GetDisposalScopeFromHttpContext(scope), null);

                HttpContext.Current.Items[uniqueId] = value;

                return (T)value;
            }

            return (T)creationDelegate(scope, scope, null);
        }
    }
}
