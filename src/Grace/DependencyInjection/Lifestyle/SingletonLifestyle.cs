using System.Linq.Expressions;
using System.Threading;

namespace Grace.DependencyInjection.Lifestyle
{
    public class SingletonLifestyle : ICompiledLifestyle
    {
        private object _singleton;
        private readonly object _lockObject = new object();

        protected Expression ConstantExpression;

        public virtual bool RootRequest { get; protected set; } = true;

        public virtual ICompiledLifestyle Clone()
        {
            return new SingletonLifestyle();
        }

        public virtual IActivationExpressionResult ProvideLifestlyExpression(IInjectionScope scope,
                                                                     IActivationExpressionRequest requst,
                                                                     IActivationExpressionResult activationExpression)
        {
            if (ConstantExpression != null)
            {
                return requst.Services.Compiler.CreateNewResult(requst, ConstantExpression);
            }
            
            var activationDelegate = requst.Services.Compiler.CompileDelegate(scope, activationExpression);

            lock (_lockObject)
            {
                if (_singleton == null)
                {
                    _singleton = activationDelegate(scope, scope, null);
                }
            }

            Interlocked.CompareExchange(ref ConstantExpression, Expression.Constant(_singleton), null);

            return requst.Services.Compiler.CreateNewResult(requst, ConstantExpression);
        }
    }
}
