using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Grace.DependencyInjection.Lifestyle
{

    /// <summary>
    /// Singleton lifestyle that defers the creation of the instance till requested.
    /// The standard singleton lifestyle creates the instance early. 
    /// </summary>
    public class DeferredSingletonLifestyle : ICompiledLifestyle
    {
        private volatile object _singleton;
        private readonly object _lockObject = new object();
        private ActivationStrategyDelegate _activationDelegate;

        /// <summary>
        /// Constant expression
        /// </summary>
        protected Expression ConstantExpression;

        public LifestyleType LifestyleType => LifestyleType.Singleton;

        /// <summary>
        /// Dependency will be created without knowledge of the object graph it's being injected into.
        /// This is true by default because it protects against weird issues that can arise if the order of a requested dependency changes.
        /// This is an advance lifestyle option and comes with risk (YMMV).
        /// </summary>
        public bool RootedRequest { get; set; } = true;

        public ICompiledLifestyle Clone()
        {
            return new DeferredSingletonLifestyle();
        }

        public IActivationExpressionResult ProvideLifestyleExpression(IInjectionScope scope,
            IActivationExpressionRequest request,
            Func<IActivationExpressionRequest, IActivationExpressionResult> activationExpression)
        {
            if (ConstantExpression != null)
            {
                return request.Services.Compiler.CreateNewResult(request, ConstantExpression);
            }

            var newRequest = request;

            if (RootedRequest)
            {
                // Create new request as we shouldn't carry over anything from the previous request
                newRequest = request.NewRootedRequest(request.ActivationType, scope, true);
            }

            _activationDelegate =
                request.Services.Compiler.CompileDelegate(scope, activationExpression(newRequest));

            var singletonMethod = GetType().GetTypeInfo().GetDeclaredMethod(nameof(SingletonActivation));

            Expression lifestyleExpression = Expression.Call(Expression.Constant(this), singletonMethod,
                request.Constants.ScopeParameter, request.Constants.RootDisposalScope,
                request.Constants.InjectionContextParameter);

            if (lifestyleExpression.Type != request.ActivationType)
            {
                lifestyleExpression = Expression.Convert(lifestyleExpression, request.ActivationType);
            }

            if (RootedRequest)
            {
                ConstantExpression = lifestyleExpression;
            }

            return request.Services.Compiler.CreateNewResult(request, lifestyleExpression);
        }

        private object SingletonActivation(IExportLocatorScope scope, IDisposalScope disposalScope,
            IInjectionContext context)
        {
            if (_singleton != null)
            {
                return _singleton;
            }

            lock (_lockObject)
            {
                if (_singleton == null)
                {
                    _singleton = _activationDelegate(scope, disposalScope, context);
                }
            }

            return _singleton;
        }
    }
}
