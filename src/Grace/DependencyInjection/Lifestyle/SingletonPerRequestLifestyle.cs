using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Grace.DependencyInjection.Lifestyle
{
    /// <summary>
    /// lifestyle for per request
    /// </summary>
    public class SingletonPerRequestLifestyle : ICompiledLifestyle
    {
        private ICompiledLifestyle _compiledLifestyle;

        /// <summary>
        /// Clone the lifestyle
        /// </summary>
        /// <returns></returns>
        public ICompiledLifestyle Clone()
        {
            return new SingletonPerRequestLifestyle();
        }

        /// <summary>
        /// Provide an expression that uses the lifestyle
        /// </summary>
        /// <param name="scope">scope for the strategy</param>
        /// <param name="request">activation request</param>
        /// <param name="activationExpression">expression to create strategy type</param>
        /// <returns></returns>
        public IActivationExpressionResult ProvideLifestlyExpression(IInjectionScope scope, IActivationExpressionRequest request,
            Func<IActivationExpressionRequest, IActivationExpressionResult> activationExpression)
        {
            if (_compiledLifestyle != null)
            {
                return _compiledLifestyle.ProvideLifestlyExpression(scope, request, activationExpression);
            }

            IPerRequestLifestyleProvider provider;

            var lifestyle = scope.TryLocate(out provider) ? 
                            provider.ProvideLifestyle() : 
                            new SingletonPerScopeLifestyle(false);

            Interlocked.CompareExchange(ref _compiledLifestyle, lifestyle, null);

            return _compiledLifestyle.ProvideLifestlyExpression(scope, request, activationExpression);
        }
    }
}
