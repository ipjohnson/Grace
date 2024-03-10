using System;
using System.Linq;
using System.Reflection;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// interface for creating enumerable locator
    /// </summary>
    public interface IDynamicIEnumerableLocator
    {
        /// <summary>
        /// Locate dynamic enumerable
        /// </summary>
        /// <param name="injectionScope"></param>
        /// <param name="scope"></param>
        /// <param name="disposalScope"></param>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <param name="consider"></param>
        /// <param name="injectionContext"></param>
        object Locate(
            IInjectionScope injectionScope, IExportLocatorScope scope, IDisposalScope disposalScope, 
            Type type, object key, 
            ActivationStrategyFilter consider, IInjectionContext injectionContext);
    }

    /// <summary>
    /// creates enumerable dynamically
    /// </summary>
    public class DynamicIEnumerableLocator : IDynamicIEnumerableLocator
    {
        /// <summary>
        /// Delegate to create enumerable
        /// </summary>
        /// <param name="key"></param>
        /// <param name="injectionScope"></param>
        /// <param name="scope"></param>
        /// <param name="disposalScope"></param>
        /// <param name="consider"></param>
        /// <param name="injectionContext"></param>
        public delegate object EnumerableCreateDelegate(
            object key,
            IInjectionScope injectionScope, 
            IExportLocatorScope scope, 
            IDisposalScope disposalScope,
            ActivationStrategyFilter consider,
            IInjectionContext injectionContext);

        private ImmutableHashTree<Type, EnumerableCreateDelegate> _delegates = ImmutableHashTree<Type, EnumerableCreateDelegate>.Empty;

        /// <summary>
        /// Locate dynamic enumerable
        /// </summary>
        /// <param name="injectionScope"></param>
        /// <param name="scope"></param>
        /// <param name="disposalScope"></param>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <param name="consider"></param>
        /// <param name="injectionContext"></param>
        public object Locate(IInjectionScope injectionScope, IExportLocatorScope scope, IDisposalScope disposalScope, Type type, object key, ActivationStrategyFilter consider, IInjectionContext injectionContext)
        {
            var createDelegate = _delegates.GetValueOrDefault(type);

            if (createDelegate == null)
            {
                var elementType = type.GenericTypeArguments[0];

                var method =
                    typeof(DynamicIEnumerableLocator).GetRuntimeMethods().FirstOrDefault(m => m.Name == nameof(EnumerableCreateMethod));

                var closedMethod = method.MakeGenericMethod(elementType);

                createDelegate = (EnumerableCreateDelegate)closedMethod.CreateDelegate(typeof(EnumerableCreateDelegate));

                createDelegate = ImmutableHashTree.ThreadSafeAdd(ref _delegates, type, createDelegate);
            }

            return createDelegate(key, injectionScope, scope, disposalScope, consider, injectionContext);
        }

        /// <summary>
        /// static method to create enumerable dynamic
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="injectionScope"></param>
        /// <param name="scope"></param>
        /// <param name="disposalScope"></param>
        /// <param name="consider"></param>
        /// <param name="injectionContext"></param>
        public static object EnumerableCreateMethod<T>(
            object key,
            IInjectionScope injectionScope, IExportLocatorScope scope,
            IDisposalScope disposalScope, ActivationStrategyFilter consider, IInjectionContext injectionContext)

        {
            var all = injectionScope.InternalLocateAll<T>(scope, disposalScope, typeof(T), key, injectionContext, consider, null);

            return injectionScope.ScopeConfiguration.Behaviors.CustomEnumerableCreator is {} enumerableCreator
                ? enumerableCreator.CreateEnumerable(scope, all.ToArray())
                : all;            
        }
    }
}
