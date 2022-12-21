using System;
using System.Threading;

namespace Grace.DependencyInjection.Lifestyle
{
    /// <summary>
    /// lifestyle for per request
    /// </summary>
    public class SingletonPerRequestLifestyle : ICompiledLifestyle
    {
        private ICompiledLifestyle _compiledLifestyle;

        /// <summary>
        /// Generalization for lifestyle
        /// </summary>
        public LifestyleType LifestyleType { get; } = LifestyleType.Scoped;

        /// <summary>
        /// Clone the lifestyle
        /// </summary>
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
        public IActivationExpressionResult ProvideLifestyleExpression(IInjectionScope scope, IActivationExpressionRequest request,
            Func<IActivationExpressionRequest, IActivationExpressionResult> activationExpression)
        {
            if (_compiledLifestyle != null)
            {
                return _compiledLifestyle.ProvideLifestyleExpression(scope, request, activationExpression);
            }

            var lifestyle = scope.TryLocate(out IPerRequestLifestyleProvider provider) ? 
                            provider.ProvideLifestyle() : 
                            new SingletonPerScopeLifestyle(false);

            Interlocked.CompareExchange(ref _compiledLifestyle, lifestyle, null);

            return _compiledLifestyle.ProvideLifestyleExpression(scope, request, activationExpression);
        }
    }
}
