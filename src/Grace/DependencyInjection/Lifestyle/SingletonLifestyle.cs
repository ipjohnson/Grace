using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading;

namespace Grace.DependencyInjection.Lifestyle
{
    /// <summary>
    /// Singleton lifestyle
    /// </summary>
    [DebuggerDisplay("Singleton Lifestyle")]
    public class SingletonLifestyle : ICompiledLifestyle
    {
        private object _singleton;
        private readonly object _lockObject = new object();
        private ActivationStrategyDelegate _activationDelegate;

        /// <summary>
        /// Constant expression
        /// </summary>
        protected Expression ConstantExpression;
        
        /// <summary>
        /// Generalization for lifestyle
        /// </summary>
        public LifestyleType LifestyleType { get; } = LifestyleType.Singleton;

        /// <summary>
        /// Clone lifestyle
        /// </summary>
        /// <returns></returns>
        public virtual ICompiledLifestyle Clone()
        {
            return new SingletonLifestyle();
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
            if (ConstantExpression != null)
            {
                return request.Services.Compiler.CreateNewResult(request, ConstantExpression);
            }

            // Create new request as we shouldn't carry over anything from the previous request
            var newRequest = request.NewRootedRequest(request.ActivationType, scope, true);
            
            _activationDelegate = request.Services.Compiler.CompileDelegate(scope, activationExpression(newRequest));

            lock (_lockObject)
            {
                if (_singleton == null)
                {
                    _singleton = _activationDelegate(scope, scope, null);
                }
            }

            Interlocked.CompareExchange(ref ConstantExpression, Expression.Constant(_singleton), null);

            return request.Services.Compiler.CreateNewResult(request, ConstantExpression);
        }
    }
}
