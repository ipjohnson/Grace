using System;
using System.Collections.Generic;
using System.Diagnostics;
using Grace.Data;
using Grace.DependencyInjection;

namespace Grace.Diagnostics
{
    /// <summary>
    /// Diagnostic class used for visual studio debugging
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ActivationStrategyCollectionContainerDebuggerView<T> where T : IActivationStrategy
    {
        private readonly List<StrategyListDebuggerView<T>> _strategiesByType;

        /// <summary>
        /// default constructor
        /// </summary>
        /// <param name="container"></param>
        public ActivationStrategyCollectionContainerDebuggerView(IActivationStrategyCollectionContainer<T> container)
        {
            _strategiesByType = new List<StrategyListDebuggerView<T>>();

            foreach (var type in container.GetActivationTypes())
            {
                var list = new List<T>();

                var collection = container.GetActivationStrategyCollection(type);

                foreach (var strategy in collection.GetStrategies())
                {
                    list.Add(strategy);
                }

                foreach (var keyedStrategy in collection.GetKeyedStrategies())
                {
                    list.Add(keyedStrategy.Value);
                }

                _strategiesByType.Add(new StrategyListDebuggerView<T>(type, list));
            }

            _strategiesByType.Sort(
                (x, y) => string.Compare(x.TypeName, y.TypeName, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// Strategies by type
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public StrategyListDebuggerView<T>[] StrategiesByType => _strategiesByType.ToArray();

        /// <summary>
        /// Strategies debugger view
        /// </summary>
        /// <typeparam name="TStrategy"></typeparam>
        [DebuggerDisplay("{" + nameof(DebuggerDisplayValue) + ",nq}", Name = "{DebuggerDisplayName,nq}")]
        public class StrategyListDebuggerView<TStrategy>
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private readonly List<TStrategy> _strategies;

            /// <summary>
            /// Default Constructor
            /// </summary>
            /// <param name="type"></param>
            /// <param name="strategies"></param>
            public StrategyListDebuggerView(Type type, List<TStrategy> strategies)
            {
                _strategies = strategies;
                Type = type;
                TypeName = ReflectionService.GetFriendlyNameForType(type, true);
            }

            /// <summary>
            /// Type of strategies
            /// </summary>
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            public Type Type { get; }

            /// <summary>
            /// Name for Type
            /// </summary>
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            public string TypeName { get; }

            /// <summary>
            /// Items
            /// </summary>
            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public IEnumerable<TStrategy> Items => _strategies;

            /// <summary>
            /// Debugger display value
            /// </summary>
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private string DebuggerDisplayValue => $"Count: {_strategies.Count}";

            /// <summary>
            /// Debugger display name
            /// </summary>
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private string DebuggerDisplayName => TypeName;
        }
    }
}