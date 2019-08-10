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
        /// <returns></returns>
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
            var requestedType = request.ActivationType;

            if (requestedType.IsConstructedGenericType)
            {
                var genericType = requestedType.GetGenericTypeDefinition();

                if (genericType == typeof(ImmutableLinkedList<>))
                {
                    yield return new ImmutableLinkListStrategy(scope);
                    yield break;
                }

                if (genericType == typeof(ImmutableArray<>))
                {
                    yield return new ImmutableArrayStrategy(scope);
                    yield break;
                }

                if (genericType == typeof(IReadOnlyCollection<>) ||
                    genericType == typeof(IReadOnlyList<>) ||
                    genericType == typeof(ReadOnlyCollection<>))
                {
                    yield return new ReadOnlyCollectionStrategy(scope);
                    yield break;
                }

                if (genericType == typeof(IList<>) ||
                    genericType == typeof(ICollection<>) ||
                    genericType == typeof(List<>))
                {
                    yield return new ListEnumerableStrategy(scope);
                    yield break;
                }

                if (genericType == typeof(KeyedLocateDelegate<,>))
                {
                    yield return new KeyedLocateDelegateStrategy(scope);
                    yield break;
                }


                if (genericType.FullName == "System.Collections.Immutable.ImmutableList`1" ||
                    genericType.FullName == "System.Collections.Immutable.ImmutableArray`1" ||
                    genericType.FullName == "System.Collections.Immutable.ImmutableQueue`1" ||
                    genericType.FullName == "System.Collections.Immutable.ImmutableStack`1" ||
                    genericType.FullName == "System.Collections.Immutable.ImmutableSortedSet`1" ||
                    genericType.FullName == "System.Collections.Immutable.ImmutableHashSet`1")
                {
                    yield return new ImmutableCollectionStrategy(genericType, scope);
                    yield break;
                }
            }

            if (requestedType.GetTypeInfo().IsInterface || 
                requestedType.GetTypeInfo().IsAbstract)
            {
                yield break;
            }

            if (typeof(MulticastDelegate).GetTypeInfo().IsAssignableFrom(requestedType.GetTypeInfo()))
            {
                var method = requestedType.GetTypeInfo().GetDeclaredMethod("Invoke");

                if (method.ReturnType != typeof(void) && 
                    scope.CanLocate(method.ReturnType))
                {
                    var parameterCount = method.GetParameters().Length;

                    switch (parameterCount)
                    {
                        case 0:
                            yield return new DelegateNoArgWrapperStrategy(requestedType, scope);
                            break;

                        case 1:
                            yield return new DelegateOneArgWrapperStrategy(requestedType, scope);
                            break;

                        case 2:
                            yield return new DelegateTwoArgWrapperStrategy(requestedType, scope);
                            break;

                        case 3:
                            yield return new DelegateThreeArgWrapperStrategy(requestedType, scope);
                            break;

                        case 4:
                            yield return new DelegateFourArgWrapperStrategy(requestedType, scope);
                            break;

                        case 5:
                            yield return new DelegateFiveArgWrapperStrategy(requestedType, scope);
                            break;
                    }
                }
            }
            else if (ShouldCreateConcreteStrategy(request))
            {
                var strategy =
                    new CompiledExportStrategy(requestedType, scope, request.Services.LifestyleExpressionBuilder).ProcessAttributeForStrategy();

                strategy.Lifestyle = scope.ScopeConfiguration.AutoRegistrationLifestylePicker?.Invoke(requestedType);

                yield return strategy;
            }
        }

        /// <summary>
        /// Should a type be exported
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldCreateConcreteStrategy(IActivationExpressionRequest request)
        {
            var type = request.ActivationType;

            if(type == typeof(string) || 
               type.GetTypeInfo().IsPrimitive)
            {
                return false;
            }

            if (type.GetTypeInfo().IsValueType && 
                (type.Namespace == "System" || (type.Namespace?.StartsWith("System") ?? false)))
            {
                return false;
            }

            if (type.IsConstructedGenericType && type.GetTypeInfo().GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return false;
            }

            return type.GetTypeInfo().DeclaredConstructors.Any(c => c.IsPublic && !c.IsStatic) && 
                   _filters.All(func => !func(type));
        }
    }
}
