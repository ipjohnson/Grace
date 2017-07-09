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
            Collections = ImmutableHashTree<Type, IActivationStrategyCollection<T>>.Empty;

            ExportAsBase = exportAsBase;
        }

        /// <summary>
        /// Protected constructor to be used internally
        /// </summary>
        /// <param name="collections"></param>
        protected ActivationStrategyCollectionContainer(
            ImmutableHashTree<Type, IActivationStrategyCollection<T>> collections)
        {
            Collections = collections;
        }

        /// <summary>
        /// Export type as their base
        /// </summary>
        protected bool ExportAsBase;


        /// <summary>
        /// Array of hash trees
        /// </summary>
        protected ImmutableHashTree<Type, IActivationStrategyCollection<T>> Collections;

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
            return Collections.Values.SelectMany(c => c.GetStrategies());
        }

        /// <summary>
        /// Get collection for a specific type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IActivationStrategyCollection<T> GetActivationStrategyCollection(Type type)
        {
            return Collections.GetValueOrDefault(type);
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
            return Collections.Keys;
        }

        /// <summary>
        /// Clone the container
        /// </summary>
        /// <returns></returns>
        public IActivationStrategyCollectionContainer<T> Clone()
        {
            return new ActivationStrategyCollectionContainer<T>(Collections);
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

            var collection = Collections.GetValueOrDefault(exportAs);

            if (collection == null)
            {
                collection = CreateCollection(exportAs);

                Collections = Collections.Add(exportAs, collection);
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

            foreach (var strategyCollection in collections.Values)
            {
                strategyCollection.Dispose();
            }
        }
    }
}
