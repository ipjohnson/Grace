using System;
using System.Collections.Generic;
using Grace.Data.Immutable;
using System.Reflection;

namespace Grace.DependencyInjection.Impl
{
    public class ActivationStrategyCollectionContainer<T> : IActivationStrategyCollectionContainer<T> where T : class, IActivationStrategy
    {
        protected bool ExportAsBase;
        protected int ArraySizeMinusOne;
        protected ImmutableHashTree<Type, IActivationStrategyCollection<T>>[] Collections;
        protected ImmutableLinkedList<IActivationStrategyInspector> Inspectors =
            ImmutableLinkedList<IActivationStrategyInspector>.Empty;

        public ActivationStrategyCollectionContainer(int arraySize, bool exportAsBase)
        {
            ArraySizeMinusOne = arraySize - 1;
            Collections = new ImmutableHashTree<Type, IActivationStrategyCollection<T>>[arraySize];

            for (var i = 0; i < arraySize; i++)
            {
                Collections[i] = ImmutableHashTree<Type, IActivationStrategyCollection<T>>.Empty;
            }

            ExportAsBase = exportAsBase;
        }

        protected ActivationStrategyCollectionContainer(
            ImmutableHashTree<Type, IActivationStrategyCollection<T>>[] collections)
        {
            ArraySizeMinusOne = collections.Length - 1;
            Collections = collections;
        }

        public void AddStrategy(T strategy)
        {
            Inspectors.Visit(inspector => inspector.Inspect(strategy), true);

            ImmutableHashTree<Type, bool> types = ImmutableHashTree<Type, bool>.Empty;

            foreach (var type in strategy.ExportAs)
            {
                if (ExportAsBase)
                {
                    foreach (var exportType in GetTypes(type))
                    {
                        types = types.Add(exportType, true, (value, newValue) => value);
                    }
                }
                else
                {
                    types = types.Add(type, true, (value, newValue) => value);
                }
            }

            bool added = false;


            foreach (var pair in types)
            {
                added = true;

                AddStrategyByAs(strategy, pair.Key, null);
            }

            foreach (var keyedPair in strategy.ExportAsKeyed)
            {
                added = true;

                if (ExportAsBase)
                {
                    foreach (var exportType in GetTypes(keyedPair.Key))
                    {
                        AddStrategyByAs(strategy, exportType, keyedPair.Value);
                    }
                }
                else
                {
                    AddStrategyByAs(strategy, keyedPair.Key, keyedPair.Value);
                }
            }

            if (!added)
            {
                if (ExportAsBase)
                {
                    foreach (var exportType in GetTypes(strategy.ActivationType))
                    {
                        AddStrategyByAs(strategy, exportType, null);
                    }
                }
                else
                {
                    AddStrategyByAs(strategy, strategy.ActivationType, null);
                }
            }
        }

        private IEnumerable<Type> GetTypes(Type type)
        {
            if (type.GetTypeInfo().IsInterface)
            {
                yield return type;

                foreach (var implementedInterface in type.GetTypeInfo().ImplementedInterfaces)
                {
                    foreach (var iType in GetTypes(implementedInterface))
                    {
                        yield return iType;
                    }
                }
            }
            else
            {
                // test for generic to see if you can construct from base
                while (type != null && type != typeof(object))
                {
                    yield return type;

                    type = type.GetTypeInfo().BaseType;
                }
            }
        }

        protected virtual IActivationStrategyCollection<T> CreateCollection(Type exportType)
        {
            return new ActivationStrategyCollection<T>(exportType);
        }

        private void AddStrategyByAs(T strategy, Type exportAs, object withKey)
        {
            var hashCode = exportAs.GetHashCode();

            var collection = Collections[hashCode & ArraySizeMinusOne].GetValueOrDefault(exportAs);

            if (collection == null)
            {
                collection = CreateCollection(exportAs);

                Collections[hashCode & ArraySizeMinusOne] =
                Collections[hashCode & ArraySizeMinusOne].Add(exportAs, collection);
            }

            collection.AddStrategy(strategy, withKey);
        }

        public IEnumerable<T> GetAllStrategies()
        {
            var returnList = new List<T>();

            foreach (var hashEntry in Collections)
            {
                hashEntry.IterateInOrder((type, export) => returnList.AddRange(export.GetStrategies()));
            }

            return returnList;
        }

        public IActivationStrategyCollection<T> GetActivationStrategyCollection(Type type)
        {
            var hashCode = type.GetHashCode();

            return Collections[hashCode & ArraySizeMinusOne].GetValueOrDefault(type, hashCode);
        }

        public IActivationStrategyCollectionContainer<T> Clone()
        {
            var newArray = new ImmutableHashTree<Type, IActivationStrategyCollection<T>>[ArraySizeMinusOne + 1];

            for (int i = 0; i <= ArraySizeMinusOne; i++)
            {
                newArray[i] = ImmutableHashTree<Type, IActivationStrategyCollection<T>>.Empty;

                foreach (var keyValuePair in Collections[i])
                {
                    newArray[i] = newArray[i].Add(keyValuePair.Key, keyValuePair.Value);
                }
            }

            return new ActivationStrategyCollectionContainer<T>(newArray);
        }

        public void AddInspector(IActivationStrategyInspector inspector)
        {
            ImmutableLinkedList.ThreadSafeAdd(ref Inspectors, inspector);

            foreach (var strategy in GetAllStrategies())
            {
                inspector.Inspect(strategy);
            }
        }
    }
}
