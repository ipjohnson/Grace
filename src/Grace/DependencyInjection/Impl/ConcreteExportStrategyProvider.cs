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
    /// Provides export strategies for concrete types
    /// </summary>
    [DebuggerDisplay("ConcreteExportStrategyProvider")]
    public class ConcreteExportStrategyProvider : IMissingExportStrategyProvider
    {
        /// <summary>
        /// Provide exports for a missing type
        /// </summary>
        /// <param name="scope">scope to provide value</param>
        /// <param name="request">request</param>
        /// <returns>set of activation strategies</returns>
        public IEnumerable<IActivationStrategy> ProvideExports(IInjectionScope scope, IActivationExpressionRequest request)
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

                if (method.ReturnType != typeof(void))
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
            else if (ShouldCreateConcreteStrategy(requestedType))
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
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual bool ShouldCreateConcreteStrategy(Type type)
        {
            if(type == typeof(string) || type.GetTypeInfo().IsPrimitive || type == typeof(DateTime))
            {
                return false;
            }

            return type.GetTypeInfo().DeclaredConstructors.Any(c => c.IsPublic && !c.IsStatic);
        }
    }
}
