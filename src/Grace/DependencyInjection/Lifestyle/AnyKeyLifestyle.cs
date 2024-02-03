using Grace.Data.Immutable;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace Grace.DependencyInjection.Lifestyle
{
    /// <summary>
    /// Lifestyle for ImportKey.Any keyed exports, that wraps the actual lifestyle.
    /// When imported, each key has its own lifestyle (e.g. singleton).
    /// </summary>
    internal sealed class AnyKeyLifestyle : ICompiledLifestyle
    {
        private class KeyedCache(ICompiledLifestyle lifestyle)
        {
            public readonly ICompiledLifestyle Lifestyle = lifestyle;
            public ActivationStrategyDelegate ActivationDelegate;
        }

        // _unkeyed serves as model for all others,
        // but also as state holder for unkeyed imports
        // (note that ImmutableHashTree doesn't accept null as key).
        private readonly KeyedCache _unkeyed;
        // Cache of activations for keyed imports.
        private volatile ImmutableHashTree<object, KeyedCache> _keyed 
            = ImmutableHashTree<object, KeyedCache>.Empty;

        public AnyKeyLifestyle(ICompiledLifestyle lifestyle)
        {
            _unkeyed = new KeyedCache(lifestyle);
        }

        /// <summary>
        /// Generalization for lifestyle
        /// </summary>
        public LifestyleType LifestyleType => _unkeyed.Lifestyle.LifestyleType;

        /// <summary>
        /// Clone the lifestyle
        /// </summary>
        public ICompiledLifestyle Clone() => new AnyKeyLifestyle(_unkeyed.Lifestyle);

        /// <summary>
        /// Provide an expression that uses the lifestyle
        /// </summary>
        /// <param name="scope">scope for the strategy</param>
        /// <param name="request">activation request</param>
        /// <param name="activationExpression">expression to create strategy type</param>
        public IActivationExpressionResult ProvideLifestyleExpression(IInjectionScope scope, IActivationExpressionRequest request,
            Func<IActivationExpressionRequest, IActivationExpressionResult> activationExpression)
        {
            // If this isn't a root activation the key is well-known and we can compile statically
            if (!request.HasDynamicKey())
            {
                return GetLifestyleForKey(request.LocateKey)
                    .Lifestyle
                    .ProvideLifestyleExpression(scope, request, activationExpression);
            }

            // For root, we compile an activation that will dynamically dispatch to the correct keyed lifestyle.
            
            // Create new request as we shouldn't carry over anything from the previous request            
            var newRequest = request.NewRootedRequest(request.ActivationType, scope, true);

            Func<ICompiledLifestyle, ActivationStrategyDelegate> delegateFactory = lifstyle =>
            {                
                var result = lifstyle.ProvideLifestyleExpression(scope, newRequest, activationExpression);
                return request.Services.Compiler.CompileDelegate(scope, result);
            };
            
            return request.Services.Compiler.CreateNewResult(
                request,
                Expression.Convert(
                    Expression.Call(
                        Expression.Constant(this),
                        typeof(AnyKeyLifestyle).GetMethod(nameof(GetInstance), BindingFlags.Instance | BindingFlags.NonPublic),
                        request.Constants.ScopeParameter,
                        request.DisposalScopeExpression,
                        request.Constants.InjectionContextParameter,
                        request.GetKeyExpression(),
                        Expression.Constant(delegateFactory)),
                    request.ActivationType)
                );
        }

        private KeyedCache GetLifestyleForKey(object key)
        {
            if (key == null)
            {
                return _unkeyed;
            }
            else
            {
                var hashmap = _keyed;

                if (hashmap.TryGetValue(key, out var cached))
                {
                    return cached;
                }

                cached = new KeyedCache(_unkeyed.Lifestyle.Clone());

                Interlocked.CompareExchange(ref _keyed, hashmap.Add(key, cached), hashmap);
                
                return _keyed[key];
            }
        }

        private object GetInstance(
            IExportLocatorScope scope,
            IDisposalScope disposalScope,
            IInjectionContext context,
            object locateKey,
            Func<ICompiledLifestyle, ActivationStrategyDelegate> delegateFactory)
        {
            var cached = GetLifestyleForKey(locateKey);

            if (cached.ActivationDelegate == null)
            {
                Interlocked.CompareExchange(ref cached.ActivationDelegate, delegateFactory(cached.Lifestyle), null);
            }

            return cached.ActivationDelegate(scope, disposalScope, context, locateKey);
        }
    }
}