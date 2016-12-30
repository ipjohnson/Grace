using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly string _uniqueId = Guid.NewGuid().ToString();

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
            throw new NotImplementedException();
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
            else
            {
                return (T)creationDelegate(scope, scope, null);
            }
        }
    }
}
