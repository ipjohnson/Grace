using System;
using System.Linq;
using System.Reflection;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Interface for creating an array that is located dynamically
    /// </summary>
    public interface IDynamicArrayLocator
    {
        /// <summary>
        /// Locate dynamic array
        /// </summary>
        /// <param name="injectionScope"></param>
        /// <param name="scope"></param>
        /// <param name="disposalScope"></param>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <param name="consider"></param>
        /// <param name="injectionContext"></param>
        object Locate(
            IInjectionScope injectionScope, 
            IExportLocatorScope scope, 
            IDisposalScope disposalScope, 
            Type type, 
            object key, 
            ActivationStrategyFilter consider, 
            IInjectionContext injectionContext);
    }

    /// <summary>
    /// Creates an array of element type 
    /// </summary>
    public class DynamicArrayLocator : IDynamicArrayLocator
    {
        /// <summary>
        /// Delegate for creating dynamic array
        /// </summary>
        /// <param name="key"></param>
        /// <param name="injectionScope"></param>
        /// <param name="scope"></param>
        /// <param name="disposalScope"></param>
        /// <param name="consider"></param>
        /// <param name="injectionContext"></param>
        public delegate object ArrayCreateDelegate(
            object key,
            IInjectionScope injectionScope, IExportLocatorScope scope, IDisposalScope disposalScope,
            ActivationStrategyFilter consider, IInjectionContext injectionContext);

        private ImmutableHashTree<Type, ArrayCreateDelegate> _delegates = ImmutableHashTree<Type, ArrayCreateDelegate>.Empty;

        /// <summary>
        /// Locate dynamic array
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
                var elementType = type.GetTypeInfo().GetElementType();

                var method =
                    typeof(DynamicArrayLocator).GetRuntimeMethods().FirstOrDefault(m => m.Name == nameof(ArrayCreateMethod));

                var closedMethod = method.MakeGenericMethod(elementType);

                createDelegate = (ArrayCreateDelegate) closedMethod.CreateDelegate(typeof(ArrayCreateDelegate));

                createDelegate = ImmutableHashTree.ThreadSafeAdd(ref _delegates, type, createDelegate);
            }

            return createDelegate(key, injectionScope, scope, disposalScope, consider, injectionContext);
        }

        /// <summary>
        /// Static method to create an a dynamic array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="injectionScope"></param>
        /// <param name="scope"></param>
        /// <param name="disposalScope"></param>
        /// <param name="consider"></param>
        /// <param name="injectionContext"></param>
        public static object ArrayCreateMethod<T>(
            object key,
            IInjectionScope injectionScope, 
            IExportLocatorScope scope,
            IDisposalScope disposalScope, 
            ActivationStrategyFilter consider,
            IInjectionContext injectionContext)
        {
            return injectionScope.InternalLocateAll<T>(scope, disposalScope, typeof(T), key, injectionContext, consider, null).ToArray();
        }
    }
}
