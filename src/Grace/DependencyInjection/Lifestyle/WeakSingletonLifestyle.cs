using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace Grace.DependencyInjection.Lifestyle
{
    public class WeakSingletonLifestyle : ICompiledLifestyle
    {
        private readonly WeakReference _weakReference = new WeakReference(null);
        private ActivationStrategyDelegate _delegate;
        private readonly object _lockObject = new object();

        public bool RootRequest { get; } = true;

        public ICompiledLifestyle Clone()
        {
            return new WeakSingletonLifestyle();
        }

        public IActivationExpressionResult ProvideLifestlyExpression(IInjectionScope scope, IActivationExpressionRequest requst,
            IActivationExpressionResult activationExpression)
        {
            if (_delegate == null)
            {
                var newDelegate = requst.Services.Compiler.CompileDelegate(scope, activationExpression);

                Interlocked.CompareExchange(ref _delegate, newDelegate, null);
            }

            var getMethod = typeof(WeakSingletonLifestyle).GetRuntimeMethod("GetValue", new[] {typeof(IInjectionScope)});

            var closedMethod = getMethod.MakeGenericMethod(requst.ActivationType);

            var expression = Expression.Call(Expression.Constant(this), closedMethod, Expression.Constant(scope));

            return requst.Services.Compiler.CreateNewResult(requst, expression);
        }

        public T GetValue<T>(IInjectionScope scope)
        {
            var target = _weakReference.Target;

            if (target != null)
            {
                return (T) target;
            }

            lock (_lockObject)
            {
                target = _weakReference.Target;

                if (target != null)
                {
                    return (T) target;
                }

                target = _delegate(scope, scope, null);

                _weakReference.Target = target;

                return (T) target;
            }
        }
    }
}
