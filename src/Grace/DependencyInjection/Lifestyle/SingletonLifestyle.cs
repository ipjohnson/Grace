using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace Grace.DependencyInjection.Lifestyle
{
    /// <summary>
    /// Singleton lifestyle
    /// </summary>
    [DebuggerDisplay("Singleton Lifestyle")]
    public class SingletonLifestyle : ICompiledLifestyle
    {
        private readonly bool _deferred;
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
        /// Dependency will be created without knowledge of the object graph it's being injected into.
        /// This is true by default because it protects against weird issues that can arise if the order of a requested dependency changes.
        /// This is an advance lifestyle option and comes with risk (YMMV).
        /// </summary>
        public bool RootedRequest { get; set; } = true;

        /// <summary>
        /// Constructor for derived classes such as DeferredSingletonLifestyle
        /// </summary>
        protected SingletonLifestyle(bool deferred) 
            => _deferred = deferred;

        /// <summary>
        /// Default constructor
        /// </summary>
        public SingletonLifestyle() 
            => _deferred = false;

        /// <summary>
        /// Clone lifestyle
        /// </summary>
        public virtual ICompiledLifestyle Clone() => new SingletonLifestyle();

        /// <summary>
        /// Provide an expression that uses the lifestyle
        /// </summary>
        /// <param name="scope">scope for the strategy</param>
        /// <param name="request">activation request</param>
        /// <param name="activationExpression">expression to create strategy type</param>
        public virtual IActivationExpressionResult ProvideLifestyleExpression(IInjectionScope scope, IActivationExpressionRequest request, Func<IActivationExpressionRequest, IActivationExpressionResult> activationExpression)
        {
            if (ConstantExpression != null)
            {
                return request.Services.Compiler.CreateNewResult(request, ConstantExpression);
            }

            lock (_lockObject)
            {
                if (ConstantExpression == null)
                {
                    // Create new request as we shouldn't carry over anything from the previous request
                    var newRequest = RootedRequest
                        ? request.NewRootedRequest(request.ActivationType, scope, true)
                        : request;
                    
                    var expression = activationExpression(newRequest);
                    _activationDelegate = request.Services.Compiler.CompileDelegate(scope, expression);

                    // For root requests the key is unknown during compilation. 
                    // If the compiled code requires the key then we need to defer the creation of the singleton.
                    if (_deferred || (newRequest.KeyRequired() && request.HasDynamicKey()))
                    {
                        var singletonMethod = typeof(SingletonLifestyle)
                            .GetMethod(
                                nameof(SingletonActivation), 
                                BindingFlags.NonPublic | BindingFlags.Instance
                            )
                            .MakeGenericMethod(request.ActivationType);

                        var rootScope = Expression.Constant(scope);

                        ConstantExpression = Expression.Call(
                            Expression.Constant(this), 
                            singletonMethod,
                            RootedRequest ? rootScope : request.Constants.ScopeParameter, 
                            rootScope,
                            request.Constants.InjectionContextParameter,
                            request.GetKeyExpression());
                    }
                    else
                    {                        
                        _singleton = _activationDelegate(scope, scope, null, request.LocateKey);
                        ConstantExpression = Expression.Constant(_singleton, request.ActivationType);
                    }
                }
            }

            return request.Services.Compiler.CreateNewResult(request, ConstantExpression);
        }

        private T SingletonActivation<T>(
            IExportLocatorScope scope, 
            IDisposalScope disposalScope,
            IInjectionContext context,
            object key)
        {
            if (_singleton == null)
            {                
                lock (_lockObject)
                {
                    if (_singleton == null)
                    {
                        _singleton = _activationDelegate(scope, disposalScope, context, key);
                        // Memory barrier to avoid exposing a partially initialized expression in ProvideLifestyleExpression
                        Volatile.Write(ref ConstantExpression, Expression.Constant(_singleton, typeof(T)));
                    }
                }
            }

            return (T)_singleton;
        }
    }
}
