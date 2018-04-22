using System;
using Grace.DependencyInjection.Impl.Expressions;
using Grace.DependencyInjection.Lifestyle;

namespace Grace.DependencyInjection.Impl.InstanceStrategies
{
    /// <summary>
    /// Base class for all delegate based strategies (i.e. FactoryOneArgStrategy etc)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DelegateBaseExportStrategy<T> : BaseInstanceExportStrategy
    {
        private readonly object _delegate;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="activationType">type being activated</param>
        /// <param name="injectionScope">injection scope</param>
        /// <param name="delegate">delegate instance</param>
        public DelegateBaseExportStrategy(Type activationType, IInjectionScope injectionScope, object @delegate) : base(activationType, injectionScope)
        {
            if (@delegate == null) throw new ArgumentNullException(nameof(@delegate));
            if (!(@delegate is Delegate)) throw new ArgumentException("parameter must be type of delegate", nameof(@delegate));

            _delegate = @delegate;
        }

        /// <summary>
        /// Create expression that is implemented in child class
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        /// <param name="lifestyle"></param>
        /// <returns></returns>
        protected override IActivationExpressionResult CreateExpression(IInjectionScope scope, IActivationExpressionRequest request,
            ICompiledLifestyle lifestyle)
        {
            if (lifestyle == null)
            {
                return CreateExpression(scope, request);
            }

            return lifestyle.ProvideLifestyleExpression(scope, request,
                expressionRequest => CreateExpression(scope, expressionRequest));
        }

        /// <summary>
        /// Create expression that calls a delegate
        /// </summary>
        /// <param name="scope">scope for the request</param>
        /// <param name="request">activation request</param>
        /// <returns></returns>
        protected virtual IActivationExpressionResult CreateExpression(IInjectionScope scope,
            IActivationExpressionRequest request)
        {
            return ExpressionUtilities.CreateExpressionForDelegate(DelegateInstance, ShouldTrackDisposable(scope), scope, request, this);
        }
        
        private bool ShouldTrackDisposable(IInjectionScope scope)
        {
            if (ExternallyOwned)
            {
                return false;
            }

            return scope.ScopeConfiguration.TrackDisposableTransients ||
                   (Lifestyle != null && Lifestyle.LifestyleType != LifestyleType.Transient);
        }

        /// <summary>
        /// Delegate to call
        /// </summary>
        protected Delegate DelegateInstance => _delegate as Delegate;
    }
}
