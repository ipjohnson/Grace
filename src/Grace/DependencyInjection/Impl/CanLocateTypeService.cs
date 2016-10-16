using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grace.Data.Immutable;

namespace Grace.DependencyInjection.Impl
{
    public interface ICanLocateTypeService
    {
        bool CanLocate(IInjectionScope injectionScope, Type type, object key = null);
    }

    public class CanLocateTypeService : ICanLocateTypeService
    {
        public bool CanLocate(IInjectionScope injectionScope, Type type, object key = null)
        {
            if (key != null)
            {
                var collection = injectionScope.StrategyCollectionContainer.GetActivationStrategyCollection(type);

                return collection.GetKeyedStrategy(key) != null;
            }

            if (injectionScope.StrategyCollectionContainer.GetActivationStrategyCollection(type) != null)
            {
                return true;
            }

            if (injectionScope.WrapperCollectionContainer.GetActivationStrategyCollection(type) != null)
            {
                return true;
            }

            if (type.IsArray)
            {
                return true;
            }

            if (type.IsConstructedGenericType)
            {
                var generic = type.GetGenericTypeDefinition();

                if (injectionScope.StrategyCollectionContainer.GetActivationStrategyCollection(generic) != null)
                {
                    return true;
                }

                if (injectionScope.WrapperCollectionContainer.GetActivationStrategyCollection(generic) != null)
                {
                    return true;
                }

                if (generic == typeof(IEnumerable<>) ||
                    generic == typeof(IList<>) ||
                    generic == typeof(ICollection<>) ||
                    generic == typeof(IReadOnlyList<>) ||
                    generic == typeof(IReadOnlyCollection<>) ||
                    generic == typeof(List<>) ||
                    generic == typeof(ReadOnlyCollection<>) ||
                    generic == typeof(ImmutableLinkedList<>) ||
                    generic == typeof(ImmutableArray<>))
                {
                    return injectionScope.ScopeConfiguration.AutoRegisterUnknown;
                }
            }

            if (!type.GetTypeInfo().IsInterface)
            {
                return injectionScope.ScopeConfiguration.AutoRegisterUnknown;
            }

            return type == typeof(ILocatorService) ||
                   type == typeof(IExportLocatorScope) ||
                   type == typeof(IInjectionContext) ||
                   type == typeof(StaticInjectionContext);
        }
    }
}
