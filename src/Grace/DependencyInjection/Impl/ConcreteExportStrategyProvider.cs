using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Grace.Data.Immutable;
using Grace.DependencyInjection.Impl.CompiledStrategies;
using Grace.DependencyInjection.Impl.EnumerableStrategies;
using Grace.DependencyInjection.Impl.KnownTypeStrategies;
using Grace.DependencyInjection.Impl.Wrappers;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Concrete Export strategy provider
    /// </summary>
    public interface IConcreteExportStrategyProvider
    {
        /// <summary>
        /// Add Filter type filter
        /// </summary>
        /// <param name="filter"></param>
        void AddFilter(Func<Type, bool> filter);
    }

    /// <summary>
    /// Provides export strategies for concrete types
    /// </summary>
    [DebuggerDisplay("ConcreteExportStrategyProvider")]
    public class ConcreteExportStrategyProvider : IMissingExportStrategyProvider, IConcreteExportStrategyProvider
    {
        private ImmutableLinkedList<Func<Type, bool>> _filters = ImmutableLinkedList<Func<Type, bool>>.Empty;

        /// <summary>
        /// Add Filter type filter
        /// </summary>
        /// <param name="filter"></param>
        public virtual void AddFilter(Func<Type, bool> filter)
        {
            if (filter != null)
            {
                _filters = _filters.Add(filter);
            }
        }

        /// <summary>
        /// Can a given request be located using this provider
        /// </summary>
        /// <param name="scope"></param>
        /// <param name="request"></param>
        public virtual bool CanLocate(IInjectionScope scope, IActivationExpressionRequest request)
        {
            var requestedType = request.ActivationType;
            
            if (requestedType.IsConstructedGenericType)
            {
                var genericType = requestedType.GetGenericTypeDefinition();

                if (genericType == typeof(ImmutableLinkedList<>))
                {
                    return true;
                }

                if (genericType == typeof(ImmutableArray<>))
                {
                    return true;
                }

                if (genericType == typeof(IReadOnlyCollection<>) ||
                    genericType == typeof(IReadOnlyList<>) ||
                    genericType == typeof(ReadOnlyCollection<>))
                {
                    return true;
                }

                if (genericType == typeof(IList<>) ||
                    genericType == typeof(ICollection<>) ||
                    genericType == typeof(List<>))
                {
                    return true;
                }

                if (genericType == typeof(KeyedLocateDelegate<,>))
                {
                    return true;
                }
  

                if (genericType.FullName == "System.Collections.Immutable.ImmutableList`1" ||
                    genericType.FullName == "System.Collections.Immutable.ImmutableArray`1" ||
                    genericType.FullName == "System.Collections.Immutable.ImmutableQueue`1" ||
                    genericType.FullName == "System.Collections.Immutable.ImmutableStack`1" ||
                    genericType.FullName == "System.Collections.Immutable.ImmutableSortedSet`1" ||
                    genericType.FullName == "System.Collections.Immutable.ImmutableHashSet`1")
                {
                    return true;
                }
            }

            if (requestedType.GetTypeInfo().IsInterface ||
                requestedType.GetTypeInfo().IsAbstract)
            {
                return false;
            }

            if (typeof(MulticastDelegate).GetTypeInfo().IsAssignableFrom(requestedType.GetTypeInfo()))
            {
                var method = requestedType.GetTypeInfo().GetDeclaredMethod("Invoke");

                if (method.ReturnType != typeof(void) &&
                    scope.CanLocate(method.ReturnType))
                {
                    return method.GetParameters().Length <= 5;
                }

                return false;
            }

            return ShouldCreateConcreteStrategy(request);
        }

        /// <summary>
        /// Provide exports for a missing type
        /// </summary>
        /// <param name="scope">scope to provide value</param>
        /// <param name="request">request</param>
        /// <returns>set of activation strategies</returns>
        public virtual IEnumerable<IActivationStrategy> ProvideExports(IInjectionScope scope, IActivationExpressionRequest request)
        {
            return GetStrategy(request.ActivationType) is {} strategy ? [strategy] : [];

            IActivationStrategy GetStrategy(Type requestedType)
            {
                if (requestedType.IsConstructedGenericType)
                {
                    var genericType = requestedType.GetGenericTypeDefinition();

                    if (genericType == typeof(ImmutableLinkedList<>))
                    {
                        return new ImmutableLinkListStrategy(scope);
                    }

                    if (genericType == typeof(ImmutableArray<>))
                    {
                         return new ImmutableArrayStrategy(scope);
                    }

                    if (genericType == typeof(IReadOnlyCollection<>) ||
                        genericType == typeof(IReadOnlyList<>) ||
                        genericType == typeof(ReadOnlyCollection<>))
                    {
                        return new ReadOnlyCollectionStrategy(scope);
                    }

                    if (genericType == typeof(IList<>) ||
                        genericType == typeof(ICollection<>) ||
                        genericType == typeof(List<>))
                    {
                        return new ListEnumerableStrategy(scope);
                    }

                    if (genericType == typeof(KeyedLocateDelegate<,>))
                    {
                        return new KeyedLocateDelegateStrategy(scope);
                    }

                    if (genericType.FullName is "System.Collections.Immutable.ImmutableList`1" 
                        or "System.Collections.Immutable.ImmutableArray`1" 
                        or "System.Collections.Immutable.ImmutableQueue`1"
                        or "System.Collections.Immutable.ImmutableStack`1"
                        or "System.Collections.Immutable.ImmutableSortedSet`1"
                        or "System.Collections.Immutable.ImmutableHashSet`1")
                    {
                        return new ImmutableCollectionStrategy(genericType, scope);
                    }
                }

                if (requestedType is { IsInterface: true } or { IsAbstract: true })
                {
                    return null;
                }

                if (typeof(MulticastDelegate).IsAssignableFrom(requestedType))
                {
                    var method = requestedType.GetMethod("Invoke");

                    if (method.ReturnType != typeof(void) && 
                        scope.CanLocate(method.ReturnType) &&
                        method.GetParameters().Length is int paramCount and < 6)
                    {
                        return paramCount switch
                        {
                            0 => new DelegateNoArgWrapperStrategy(requestedType, scope),
                            1 => new DelegateOneArgWrapperStrategy(requestedType, scope),
                            2 => new DelegateTwoArgWrapperStrategy(requestedType, scope),
                            3 => new DelegateThreeArgWrapperStrategy(requestedType, scope),
                            4 => new DelegateFourArgWrapperStrategy(requestedType, scope),
                            5 => new DelegateFiveArgWrapperStrategy(requestedType, scope),
                            _ => throw new InvalidOperationException("Unreachable code"),
                        };
                    }
                }
                else if (ShouldCreateConcreteStrategy(request))
                {
                    var strategy = new CompiledExportStrategy(requestedType, scope, request.Services.LifestyleExpressionBuilder)
                        .ProcessAttributeForStrategy();

                    strategy.Lifestyle = scope.ScopeConfiguration.AutoRegistrationLifestylePicker?.Invoke(requestedType);

                    return strategy;
                }

                return null;
            }
        }

        /// <summary>
        /// Should a type be exported
        /// </summary>
        public virtual bool ShouldCreateConcreteStrategy(IActivationExpressionRequest request)
        {
            var type = request.ActivationType;

            if(type == typeof(string) || type.IsPrimitive)
            {
                return false;
            }

            if (type.IsValueType && 
                (type.Namespace == "System" || (type.Namespace?.StartsWith("System") ?? false)))
            {
                return false;
            }

            if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return false;
            }

            return type.GetConstructors().Any(c => c.IsPublic && !c.IsStatic) && 
                   _filters.All(func => !func(type));
        }
    }
}
