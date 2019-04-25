using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using Grace.Data.Immutable;
using Grace.Diagnostics;

namespace Grace.DependencyInjection.Impl
{
    /// <summary>
    /// Container of activation strategy collection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DebuggerDisplay("{" + nameof(DebugDisplayString) + ",nq}")]
    [DebuggerTypeProxy(typeof(ActivationStrategyCollectionContainerDebuggerView<>))]
    public class ActivationStrategyCollectionContainer<T> : IActivationStrategyCollectionContainer<T> where T : class, IActivationStrategy
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="arraySize"></param>
        /// <param name="exportAsBase"></param>
        public ActivationStrategyCollectionContainer(int arraySize, bool exportAsBase)
        {
            ArrayLengthMinusOne = arraySize - 1;
            Collections = new ImmutableHashTree<Type, IActivationStrategyCollection<T>>[arraySize];

            for (var i = 0; i < arraySize; i++)
            {
                Collections[i] = ImmutableHashTree<Type, IActivationStrategyCollection<T>>.Empty;
            }

            ExportAsBase = exportAsBase;
        }

        /// <summary>
        /// Protected constructor to be used internally
        /// </summary>
        /// <param name="collections"></param>
        protected ActivationStrategyCollectionContainer(
            ImmutableHashTree<Type, IActivationStrategyCollection<T>>[] collections)
        {
            ArrayLengthMinusOne = collections.Length - 1;
            Collections = collections;
        }

        /// <summary>
        /// Export type as their base
        /// </summary>
        protected bool ExportAsBase;

        /// <summary>
        /// Array length of Collections minus one 
        /// </summary>
        protected int ArrayLengthMinusOne;

        /// <summary>
        /// Array of hash trees
        /// </summary>
        protected ImmutableHashTree<Type, IActivationStrategyCollection<T>>[] Collections;

        /// <summary>
        /// Inspectors to apply to strategies
        /// </summary>
        protected ImmutableLinkedList<IActivationStrategyInspector> Inspectors =
            ImmutableLinkedList<IActivationStrategyInspector>.Empty;

        /// <summary>
        /// Strategies by name
        /// </summary>
        protected ImmutableHashTree<string, IActivationStrategyCollection<T>> StrategiesByName =
            ImmutableHashTree<string, IActivationStrategyCollection<T>>.Empty;

        /// <summary>
        /// Add strategy to container
        /// </summary>
        /// <param name="strategy">strategy</param>
        public void AddStrategy(T strategy)
        {
            if (strategy.ExportOrder == 0)
            {
                strategy.ExportOrder = ActivationStrategyCollectionContainerCounter.IncrementCount();
            }

            Inspectors.Visit(inspector => inspector.Inspect(strategy), true);

            var types = ImmutableHashTree<Type, bool>.Empty;

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

            var added = false;

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

            foreach (var name in strategy.ExportAsName)
            {
                added = true;

                AddStrategyByName(strategy, name);
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

        /// <summary>
        /// Get all strategies
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> GetAllStrategies()
        {
            var returnList = new Dictionary<int,T>();

            foreach (var hashEntry in Collections)
            {
                hashEntry.IterateInOrder((type, export) =>
                {
                    foreach (var strategy in export.GetStrategies())
                    {
                        returnList[strategy.ExportOrder] = strategy;
                    }

                    foreach (var valuePair in export.GetKeyedStrategies())
                    {
                        returnList[valuePair.Value.ExportOrder] = valuePair.Value;
                    }
                });
            }

            return returnList.Values;
        }

        /// <summary>
        /// Get collection for a specific type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IActivationStrategyCollection<T> GetActivationStrategyCollection(Type type)
        {
            var hashCode = type.GetHashCode();

            return Collections[hashCode & ArrayLengthMinusOne].GetValueOrDefault(type, hashCode);
        }

        /// <summary>
        /// Get collection for a specific name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IActivationStrategyCollection<T> GetActivationStrategyCollectionByName(string name)
        {
            return StrategiesByName.GetValueOrDefault(name);
        }

        /// <summary>
        /// Get all activation types
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Type> GetActivationTypes()
        {
            var returnList = ImmutableLinkedList<Type>.Empty;

            foreach (var hashEntry in Collections)
            {
                hashEntry.IterateInOrder((type, collection) => returnList = returnList.Add(type));
            }

            return returnList;
        }

        /// <summary>
        /// Clone the container
        /// </summary>
        /// <returns></returns>
        public IActivationStrategyCollectionContainer<T> Clone()
        {
            var newArray = new ImmutableHashTree<Type, IActivationStrategyCollection<T>>[ArrayLengthMinusOne + 1];

            for (var i = 0; i <= ArrayLengthMinusOne; i++)
            {
                newArray[i] = ImmutableHashTree<Type, IActivationStrategyCollection<T>>.Empty;

                foreach (var keyValuePair in Collections[i])
                {
                    newArray[i] = newArray[i].Add(keyValuePair.Key, keyValuePair.Value.Clone());
                }
            }

            return new ActivationStrategyCollectionContainer<T>(newArray);
        }

        /// <summary>
        /// Add strategy inspector
        /// </summary>
        /// <param name="inspector">inspector</param>
        public void AddInspector(IActivationStrategyInspector inspector)
        {
            ImmutableLinkedList.ThreadSafeAdd(ref Inspectors, inspector);

            foreach (var strategy in GetAllStrategies())
            {
                inspector.Inspect(strategy);
            }
        }

        /// <summary>
        /// Create a collection of activation strategy
        /// </summary>
        /// <param name="exportType"></param>
        /// <returns></returns>
        protected virtual IActivationStrategyCollection<T> CreateCollection(Type exportType)
        {
            return new ActivationStrategyCollection<T>(exportType);
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

        private void AddStrategyByName(T strategy, string name)
        {
            var collection = StrategiesByName.GetValueOrDefault(name);

            if (collection == null)
            {
                collection = new ActivationStrategyCollection<T>(typeof(object));

                StrategiesByName = StrategiesByName.Add(name, collection);
            }

            collection.AddStrategy(strategy, null);
        }

        private void AddStrategyByAs(T strategy, Type exportAs, object withKey)
        {
            var hashCode = exportAs.GetHashCode();

            var collection = Collections[hashCode & ArrayLengthMinusOne].GetValueOrDefault(exportAs);

            if (collection == null)
            {
                collection = CreateCollection(exportAs);

                Collections[hashCode & ArrayLengthMinusOne] =
                Collections[hashCode & ArrayLengthMinusOne].Add(exportAs, collection);
            }

            collection.AddStrategy(strategy, withKey);
        }

        private string DebugDisplayString => "Count: " + GetAllStrategies().Count();

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            var collections = Interlocked.Exchange(ref Collections, null);

            if (collections == null)
            {
                return;
            }

            foreach (var collection in collections)
            {
                foreach (var strategyCollection in collection.Values)
                {
                    strategyCollection.Dispose();
                }
            }
        }
    }

    /// <summary>
    /// Class to wrap a counter for all exports including wrappers and decorators
    /// </summary>
    public class ActivationStrategyCollectionContainerCounter
    {
        /// <summary>
        /// 
        /// </summary>
        private static int _totalExportCount = 0;

        /// <summary>
        /// Increment the count of total 
        /// </summary>
        /// <returns></returns>
        public static int IncrementCount()
        {
            return Interlocked.Increment(ref _totalExportCount);
        }
    }
}
