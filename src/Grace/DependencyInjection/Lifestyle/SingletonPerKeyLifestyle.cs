using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Lifestyle
{
    public class SingletonPerKeyLifestyle : ICompiledLifestyle
    {
        private static MethodInfo getInstanceMethodInfo =
            typeof(SingletonPerKeyLifestyle).GetTypeInfo().GetDeclaredMethod(nameof(GetInstance));

        private Func<IExportLocatorScope, IInjectionContext, object> _keyFunc;
        private ImmutableHashTree<object,object> _singletons = ImmutableHashTree<object, object>.Empty;
        private ActivationStrategyDelegate _activationDelegate;

        /// <summary>
        /// Constant expression
        /// </summary>
        protected Expression ConstantExpression;

        public SingletonPerKeyLifestyle(Func<IExportLocatorScope, IInjectionContext, object> keyFunc)
        {
            _keyFunc = keyFunc;
        }

        public LifestyleType LifestyleType => LifestyleType.Scoped;

        public ICompiledLifestyle Clone()
        {
            return new SingletonPerKeyLifestyle(_keyFunc);
        }

        public IActivationExpressionResult ProvideLifestyleExpression(IInjectionScope scope, IActivationExpressionRequest request,
            Func<IActivationExpressionRequest, IActivationExpressionResult> activationExpression)
        {
            request.RequireInjectionContext();

            if (ConstantExpression != null)
            {
                return request.Services.Compiler.CreateNewResult(request, ConstantExpression);
            }

            // Create new request as we shouldn't carry over anything from the previous request
            var newRequest = request.NewRootedRequest(request.ActivationType, scope, true);

            _activationDelegate = request.Services.Compiler.CompileDelegate(scope, activationExpression(newRequest));

            ConstantExpression = Expression.Convert(Expression.Call(Expression.Constant(this), getInstanceMethodInfo,
                newRequest.ScopeParameter, Expression.Constant(scope), request.InjectionContextParameter,
                Expression.Constant(_activationDelegate)), request.ActivationType);

            var result = request.Services.Compiler.CreateNewResult(request, ConstantExpression);

            return result;
        }

        private object GetInstance(IExportLocatorScope scope, IDisposalScope disposalScope, IInjectionContext context,
            ActivationStrategyDelegate activationDelegate)
        {
            var key = _keyFunc(scope, context);

            var value = _singletons.GetValueOrDefault(key);

            if (value != null)
            {
                return value;
            }

            value = activationDelegate(scope, disposalScope, context);

            value = ImmutableHashTree.ThreadSafeAdd(ref _singletons, key, value);

            return value;
        }
    }
}
