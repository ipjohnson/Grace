using System;
using System.Linq;
using System.Reflection;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Inteface for creating an array that is located dynamically
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
        /// <param name="consider"></param>
        /// <param name="injectionContext"></param>
        /// <returns></returns>
        object Locate(IInjectionScope injectionScope, IExportLocatorScope scope, IDisposalScope disposalScope, Type type, ActivationStrategyFilter consider, IInjectionContext injectionContext);
    }

    /// <summary>
    /// Creates an array of element type 
    /// </summary>
    public class DynamicArrayLocator : IDynamicArrayLocator
    {
        /// <summary>
        /// Delegate for creating dynamic array
        /// </summary>
        /// <param name="injectionScope"></param>
        /// <param name="scope"></param>
        /// <param name="disposalScope"></param>
        /// <param name="consider"></param>
        /// <param name="injectionContext"></param>
        /// <returns></returns>
        public delegate object ArrayCreateDelegate(
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
        /// <param name="consider"></param>
        /// <param name="injectionContext"></param>
        /// <returns></returns>
        public object Locate(IInjectionScope injectionScope, IExportLocatorScope scope, IDisposalScope disposalScope, Type type, ActivationStrategyFilter consider, IInjectionContext injectionContext)
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

            return createDelegate(injectionScope, scope, disposalScope, consider, injectionContext);
        }

        /// <summary>
        /// Static method to create an a dynamic array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="injectionScope"></param>
        /// <param name="scope"></param>
        /// <param name="disposalScope"></param>
        /// <param name="consider"></param>
        /// <param name="injectionContext"></param>
        /// <returns></returns>
        public static object ArrayCreateMethod<T>(IInjectionScope injectionScope, IExportLocatorScope scope,
            IDisposalScope disposalScope, ActivationStrategyFilter consider,
            IInjectionContext injectionContext)
        {
            return injectionScope.InternalLocateAll<T>(scope, disposalScope, typeof(T), injectionContext, consider, null).ToArray();
        }
    }
}
