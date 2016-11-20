using System;
using System.Reflection;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl.Expressions
{
    public interface IWrapperExpressionCreator
    {
        IActivationExpressionResult GetActivationStrategy(IInjectionScope scope, IActivationExpressionRequest request);

        ImmutableLinkedList<IActivationPathNode> GetWrappers(IInjectionScope scope, Type type, IActivationExpressionRequest request, out Type wrappedType);
    }

    public class WrapperExpressionCreator : IWrapperExpressionCreator
    {
        public IActivationExpressionResult GetActivationStrategy(IInjectionScope scope, IActivationExpressionRequest request)
        {
            Type wrappedType;

            var wrappers = GetWrappers(scope, request.ActivationType, request, out wrappedType);

            if (wrappers != ImmutableLinkedList<IActivationPathNode>.Empty)
            {
                if (request.DecoratorPathNode != null &&
                    wrappedType.GetTypeInfo().IsAssignableFrom(request.DecoratorPathNode.Strategy.ActivationType.GetTypeInfo()))
                {
                    var decorator = request.PopDecoratorPathNode();

                    wrappers = ImmutableLinkedList<IActivationPathNode>.Empty.Add(decorator).AddRange(wrappers.Reverse());
                }
                else
                {
                    var collection = scope.StrategyCollectionContainer.GetActivationStrategyCollection(wrappedType);

                    if (collection == null && wrappedType.IsConstructedGenericType)
                    {
                        var generic = wrappedType.GetGenericTypeDefinition();

                        collection = scope.StrategyCollectionContainer.GetActivationStrategyCollection(generic);
                    }

                    if (collection != null)
                    {
                        var primary = request.Filter == null ? collection.GetPrimary() : null;

                        if (primary != null)
                        {
                            wrappers = ImmutableLinkedList<IActivationPathNode>.Empty.Add(new WrapperActivationPathNode(primary, wrappedType, null)).AddRange(wrappers.Reverse());
                        }
                        else
                        {
                            foreach (var strategy in collection.GetStrategies())
                            {
                                bool pass = true;

                                if (strategy.HasConditions)
                                {
                                    foreach (var condition in strategy.Conditions)
                                    {
                                        if (!condition.MeetsCondition(strategy, request.GetStaticInjectionContext()))
                                        {
                                            pass = false;
                                            break;
                                        }
                                    }
                                }

                                if (pass &&
                                    (request.Filter == null || request.Filter(strategy)))
                                {
                                    wrappers = ImmutableLinkedList<IActivationPathNode>.Empty.Add(new WrapperActivationPathNode(primary, wrappedType, null)).AddRange(wrappers.Reverse());
                                }
                            }
                        }
                    }
                    else
                    {
                        return null;
                    }
                }

                request.SetWrapperPath(wrappers);

                var wrapper = request.PopWrapperPathNode();

                return wrapper.GetActivationExpression(scope, request);
            }

            return null;
        }

        public ImmutableLinkedList<IActivationPathNode> GetWrappers(IInjectionScope scope, Type type, IActivationExpressionRequest request, out Type wrappedType)
        {
            var wrapperCollection = scope.WrapperCollectionContainer.GetActivationStrategyCollection(type);

            if (wrapperCollection == null && type.IsConstructedGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();

                wrapperCollection = scope.WrapperCollectionContainer.GetActivationStrategyCollection(genericType);
            }

            if (wrapperCollection != null)
            {
                var strategy = request.Filter == null ? wrapperCollection.GetPrimary() : null;

                if (strategy == null)
                {
                    foreach (var s in wrapperCollection.GetStrategies())
                    {
                        bool pass = true;
                        if (s.HasConditions)
                        {
                            foreach (var condition in s.Conditions)
                            {
                                if (!condition.MeetsCondition(s, request.GetStaticInjectionContext()))
                                {
                                    pass = false;
                                }
                            }
                        }

                        if (pass)
                        {
                            strategy = s;
                            break;
                        }
                    }
                }

                if (strategy != null)
                {
                    var newType = strategy.GetWrappedType(type);
                    
                    if (newType == null)
                    {
                        throw new Exception("Wrapper strategy returned null for wrapped type, " +
                                            strategy.GetType().FullName);
                    }

                    return GetWrappers(scope, newType, request, out wrappedType).Add(new WrapperActivationPathNode(strategy, type, null));
                }
            }

            wrappedType = type;

            return ImmutableLinkedList<IActivationPathNode>.Empty;
        }
    }
}
