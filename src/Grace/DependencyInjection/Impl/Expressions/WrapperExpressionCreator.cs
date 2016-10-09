using System;
using System.Reflection;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl.Expressions
{
    public interface IWrapperExpressionCreator
    {
        IActivationExpressionResult GetActivationStrategy(IInjectionScope scope, IActivationExpressionRequest request);

        ImmutableLinkedList<IActivationPathNode> GetWrappers(IInjectionScope scope, Type type, out Type wrappedType);
    }

    public class WrapperExpressionCreator : IWrapperExpressionCreator
    {
        public IActivationExpressionResult GetActivationStrategy(IInjectionScope scope, IActivationExpressionRequest request)
        {
            Type wrappedType;

            var wrappers = GetWrappers(scope, request.ActivationType, out wrappedType);

            if (wrappers != ImmutableLinkedList<IActivationPathNode>.Empty)
            {
                if (request.DecoratorPathNode != null && wrappedType.GetTypeInfo().IsAssignableFrom(request.DecoratorPathNode.Strategy.ActivationType.GetTypeInfo()))
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
                        var primary = collection.GetPrimary();

                        if (primary != null)
                        {
                            wrappers = ImmutableLinkedList<IActivationPathNode>.Empty.Add(new WrapperActivationPathNode ( primary, wrappedType, null)).AddRange(wrappers.Reverse());
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }

                request.SetWrapperPath(wrappers);

                var wrapper = request.PopWrapperPathNode();

                return wrapper.GetActivationExpression(scope, request);
            }

            return null;
        }

        public ImmutableLinkedList<IActivationPathNode> GetWrappers(IInjectionScope scope, Type type, out Type wrappedType)
        {
            var wrapperCollection = scope.WrapperCollectionContainer.GetActivationStrategyCollection(type);

            if (wrapperCollection == null && type.IsConstructedGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();

                wrapperCollection = scope.WrapperCollectionContainer.GetActivationStrategyCollection(genericType);
            }

            if (wrapperCollection != null)
            {
                var primary = wrapperCollection.GetPrimary();

                if (primary != null)
                {
                    var newType = primary.GetWrappedType(type);

                    if (newType == null)
                    {
                        throw new Exception("Wrapper strategy returned null for wrapped type, " + primary.GetType().FullName);
                    }
                    return GetWrappers(scope, newType, out wrappedType).Add(new WrapperActivationPathNode ( primary, type, null));
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            wrappedType = type;

            return ImmutableLinkedList<IActivationPathNode>.Empty;
        }
    }
}
