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

        /// <summary>
        /// Constant expression
        /// </summary>
        protected Expression ConstantExpression;

        /// <summary>
        /// Root the request context when creating expression
        /// </summary>
        public virtual bool RootRequest { get; protected set; } = true;

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
        public virtual IActivationExpressionResult ProvideLifestlyExpression(IInjectionScope scope,
                                                                     IActivationExpressionRequest request,
                                                                     IActivationExpressionResult activationExpression)
        {
            if (ConstantExpression != null)
            {
                return request.Services.Compiler.CreateNewResult(request, ConstantExpression);
            }
            
            var activationDelegate = request.Services.Compiler.CompileDelegate(scope, activationExpression);

            lock (_lockObject)
            {
                if (_singleton == null)
                {
                    _singleton = activationDelegate(scope, scope, null);
                }
            }

            Interlocked.CompareExchange(ref ConstantExpression, Expression.Constant(_singleton), null);

            return request.Services.Compiler.CreateNewResult(request, ConstantExpression);
        }
    }
}
